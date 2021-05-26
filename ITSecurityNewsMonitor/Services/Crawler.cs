using Hangfire.Server;
using ITSecurityNewsMonitor.Data;
using ITSecurityNewsMonitor.Helper;
using ITSecurityNewsMonitor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.Services
{
    public class Crawler
    {
        private static readonly HttpClient _client = new HttpClient()
        {
            Timeout = TimeSpan.FromMinutes(30)
        };
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;

        private double threshold = 0.8;
        private string _url;
        private bool migrateData = true;


        IServiceProvider _serviceProvider;
        public Crawler(IServiceProvider serviceProvider, IConfiguration config, IMemoryCache memoryCache, ILogger<Crawler> logger)
        {
            _serviceProvider = serviceProvider;
            _config = config;
            _cache = memoryCache;
            _logger = logger;

            _url = "http://" + _config.GetValue<string>("Connections:Crawler:IP") + ":" + _config.GetValue<string>("Connections:Crawler:Port");
        }

        public async Task DeleteOld(PerformContext pc)
        {
            using (HangfireConsoleLogger.InContext(pc))
            {
                _logger.LogInformation("Starting the archiving of old stories");

                using (IServiceScope scope = _serviceProvider.CreateScope())
                using (SecNewsDbContext context = scope.ServiceProvider.GetRequiredService<SecNewsDbContext>())
                {
                    DateTime thiryDaysAgo = DateTime.Now.AddDays(-30);
                    List<NewsGroup> newsGroups = context.NewsGroups.Where(ng => ng.UpdatedDate < thiryDaysAgo).ToList();

                    foreach (NewsGroup newsGroup in newsGroups)
                    {
                        newsGroup.Archived = true;
                    }

                    context.SaveChanges();
                    _logger.LogInformation("Archived " + newsGroups.Count() + " stories that were older than 30 days");
                }
            }
        }

        public async Task MigrateData(PerformContext pc)
        {
            using (HangfireConsoleLogger.InContext(pc))
            {
                try { if (!migrateData)
                    {
                        _logger.LogInformation("Skipping data migration");
                        return;
                    }

                    // write a migration logic here
                    using (IServiceScope scope = _serviceProvider.CreateScope())
                    using (SecNewsDbContext context = scope.ServiceProvider.GetRequiredService<SecNewsDbContext>())
                    {
                        List<NewsGroup> newsGroups = context.NewsGroups.ToList();
                        int newsGroupCount = newsGroups.Count();
                        context.NewsGroups.RemoveRange(newsGroups);

                        List<News> news = context.News.ToList();
                        List<News> notDuplicates = new List<News>();
                        List<News> duplicates = new List<News>();
                        foreach (News ne in news)
                        {
                            if (notDuplicates.Where(n => n.Link.Equals(ne.Link)).Any())
                            {
                                duplicates.Add(ne);
                            } else
                            {
                                notDuplicates.Add(ne);
                                ne.AssignedToStory = false;
                            }
                        }

                        context.News.RemoveRange(duplicates);

                        context.SaveChanges();
                        _logger.LogInformation("Deleted " + newsGroupCount + " stories");
                        _logger.LogInformation("Deleted " + duplicates.Count() + " dupicate news");
                        _logger.LogInformation("Cleared the similaritiy scores for " + notDuplicates.Count() + " stories");
                    }
                } catch (Exception e)
                {
                    _logger.LogError("Issue in data migration (crawler).");
                    _logger.LogError(e.Message);
                }
            }
        }

        public async Task ExecuteCrawl(PerformContext pc)
        {
            using (HangfireConsoleLogger.InContext(pc))
            {
                try
                {
                    _logger.LogInformation("Started crawling for new news");

                    using (IServiceScope scope = _serviceProvider.CreateScope())
                    using (SecNewsDbContext context = scope.ServiceProvider.GetRequiredService<SecNewsDbContext>())
                    {
                        List<Tag> tags = context.Tags.ToList();
                        List<Source> sources = context.Sources.ToList();
                        _logger.LogInformation("Found " + sources.Count() + " sources to be crawled");

                        // Get new articles
                        foreach (Source source in sources)
                        {
                            _logger.LogInformation("Started crawling " + source.Name);

                            List<Entry> entries = await GetEntries(source);

                            _logger.LogInformation("Found " + entries.Count() + " news");

                            foreach (Entry entry in entries)
                            {
                                if (!context.News.Where(n => n.Link.Equals(entry.Link)).Any())
                                { // no news article matching to the RSS entry in DB
                                    News news = new News();
                                    news.Headline = entry.Headline;
                                    news.Link = entry.Link;
                                    news.CreatedDate = DateTime.Now;
                                    news.Summary = entry.Summary;
                                    news.Content = entry.Content;
                                    news.AssignedToStory = false;
                                    news.Source = source;
                                    news.ManuallyAssigned = false;

                                    /*List<string> foundTags = await ExtractTags(news.Content, tags);

                                    news.Tags = context.Tags.Where(tag => foundTags.Contains(tag.Name)).ToList();*/
                                    news.Tags = new List<Tag>();

                                    context.News.Add(news);
                                    _logger.LogInformation("Added new news: " + news.Headline);
                                }
                            }
                        }

                        context.SaveChanges();
                        _logger.LogInformation("Saved new news articles to DB");

                        if (context.News.Any())
                        {
                            _logger.LogInformation("Started check for similarity");
                            DateTime ninetyDaysAgo = DateTime.Now.AddDays(-90);
                            // check similarity for anything that is new comparing it to any news assigned to a news group that was updated in the last 90 days.
                            Dictionary<int, Dictionary<int, double>> similarities = await ComputeSimilarities(context.News.Include(n => n.NewsGroups).Where(n => n.AssignedToStory != true || n.NewsGroups.Any(ng => ninetyDaysAgo < ng.UpdatedDate)).ToList());

                            foreach (KeyValuePair<int, Dictionary<int, double>> similarity in similarities)
                            {
                                News news = context.News.Where(n => n.AssignedToStory == false && n.ID == similarity.Key).FirstOrDefault();
                                if (news == null)
                                {
                                    continue;
                                }

                                _logger.LogInformation("Computing similarity for " + news.ID);

                                List<NewsGroup> similarNewsGroups = context.NewsGroups
                                    .Where(ng => ninetyDaysAgo < ng.UpdatedDate)
                                    .Include(ng => ng.News)
                                    .ToList()
                                    .Select(ng => new
                                    {
                                        NewsGroup = ng,
                                        SimilarityScore = ng.News.Average(n => similarity.Value[n.ID])
                                    })
                                    .Where(ng => ng.SimilarityScore > threshold)
                                    .Select(ng => ng.NewsGroup)
                                    .ToList();

                                _logger.LogInformation("Found " + similarNewsGroups.Count() + " similar stories");

                                if (similarNewsGroups.Any()) // the article will be added to existing groups
                                {
                                    foreach (NewsGroup newsGroup in similarNewsGroups)
                                    {
                                        newsGroup.News.Add(news);
                                    }
                                    _logger.LogInformation("Assigned news " + news.ID + " to stories.");
                                }
                                else // a new group should be created
                                {
                                    NewsGroup newsGroup = new NewsGroup();
                                    newsGroup.Score = 0;
                                    newsGroup.CreatedDate = DateTime.Now;
                                    newsGroup.UpdatedDate = DateTime.Now;
                                    newsGroup.News = new List<News>();

                                    newsGroup.News.Add(news);

                                    context.NewsGroups.Add(newsGroup);
                                    _logger.LogInformation("Create new story");
                                }

                                news.AssignedToStory = true;

                                context.SaveChanges();
                                _logger.LogInformation("Saved changes in the DB");
                            }
                        }
                    }
                } catch (Exception e)
                {
                    _logger.LogError("Error in crawler.");
                    _logger.LogError(e.Message);
                }
            }
        }

        public async Task<List<Entry>> GetEntries(Source source)
        {
            var input = System.Text.Json.JsonSerializer.Serialize(new
            {
                url = source.Link
            }, _options);

            var content = new StringContent(input, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(_url + "/rss", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                List<Entry> responseObject = System.Text.Json.JsonSerializer.Deserialize<List<Entry>>(responseContent);

                return responseObject;
            }
            else
            {
                throw new System.ArgumentException("API returned status code: " + response.StatusCode);
            }
        }

        public async Task<List<string>> ExtractTags(string text, List<Tag> tags)
        {
            var input = System.Text.Json.JsonSerializer.Serialize(new
            {
                text = text,
                keywords = tags.Select(tag => tag.Name).ToList()
            }, _options);

            var content = new StringContent(input, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(_url + "/keywords", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                List<string> responseObject = System.Text.Json.JsonSerializer.Deserialize<List<string>>(responseContent);

                return responseObject;
            }
            else
            {
                throw new System.ArgumentException("API returned status code: " + response.StatusCode);
            }
        }

        public async Task ComputeSimilarity(string id, News news1, News news2)
        {
            if (news1 != null && news2 != null)
            {
                var input = System.Text.Json.JsonSerializer.Serialize(new
                {
                    text1 = news1.Content,
                    text2 = news2.Content
                }, _options);

                var content = new StringContent(input, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(_url + "/similarity", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    double responseObject = JsonConvert.DeserializeObject<double>(responseContent);

                    var cacheExpiryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                        Priority = CacheItemPriority.High,
                        SlidingExpiration = TimeSpan.FromMinutes(2),
                        Size = 1024,
                    };
                    _cache.Set(id, responseObject.ToString(), cacheExpiryOptions);

                    string value = string.Empty;
                    _cache.TryGetValue(id, out value);
                }
                else
                {
                    throw new System.ArgumentException("API returned status code: " + response.StatusCode);
                }
                
            }
        }

        public async Task<Dictionary<int, Dictionary<int, double>>> ComputeSimilarities(List<News> news)
        {
            var input = System.Text.Json.JsonSerializer.Serialize(new
            {
                articles = news.Select(n => new
                {
                    id = n.ID,
                    text = n.Content
                })
            }, _options);

            var content = new StringContent(input, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(_url + "/similarities", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Dictionary<int, Dictionary<int, double>> responseObject = JsonConvert.DeserializeObject <Dictionary<int, Dictionary<int, double>>> (responseContent);

                return responseObject;
            }
            else
            {
                throw new System.ArgumentException("API returned status code: " + response.StatusCode);
            }
        }
    }
}