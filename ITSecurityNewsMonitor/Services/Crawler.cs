using ITSecurityNewsMonitor.Data;
using ITSecurityNewsMonitor.Helper;
using ITSecurityNewsMonitor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        private double threshold = 0.75;
        private string _url;

        IServiceProvider _serviceProvider;
        public Crawler(IServiceProvider serviceProvider, IConfiguration config, IMemoryCache memoryCache)
        {
            _serviceProvider = serviceProvider;
            _config = config;
            _cache = memoryCache;

            _url = "http://" + _config.GetValue<string>("Connections:Crawler:IP") + ":" + _config.GetValue<string>("Connections:Crawler:Port");
        }

        public async Task DeleteOld()
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            using (SecNewsDbContext context = scope.ServiceProvider.GetRequiredService<SecNewsDbContext>())
            {
                DateTime thiryDaysAgo = DateTime.Now.AddDays(-30);
                List<NewsGroup> newsGroups = context.NewsGroups.Where(ng => ng.UpdatedDate < thiryDaysAgo).ToList();

                foreach(NewsGroup newsGroup in newsGroups)
                {
                    newsGroup.Archived = true;
                }

                context.SaveChanges();
            }
        }

        public async Task ExecuteCrawl()
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            using (SecNewsDbContext context = scope.ServiceProvider.GetRequiredService<SecNewsDbContext>())
            {
                List<Tag> tags = context.Tags.ToList();
                List<Source> sources = context.Sources.ToList();
                // Get new articles
                foreach (Source source in sources)
                {
                    List<Entry> entries = await GetEntries(source);

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
                        }
                    }
                }

                context.SaveChanges();

                if (context.News.Any())
                {
                    DateTime ninetyDaysAgo = DateTime.Now.AddDays(-90);
                    // check similarity for anything that is new comparing it to any news assigned to a news group that was updated in the last 90 days.
                    Dictionary<int, Dictionary<int, double>> similarities = await ComputeSimilarities(context.News.Include(n => n.NewsGroups).Where(n => n.AssignedToStory != true || n.NewsGroups.Any(ng => ninetyDaysAgo < ng.UpdatedDate)).ToList());

                    foreach (KeyValuePair<int, Dictionary<int, double>> similarity in similarities)
                    {
                        News news = context.News.Where(n => n.AssignedToStory == false && n.ID == similarity.Key).FirstOrDefault();
                        if(news == null)
                        {
                            continue;
                        }

                        List<NewsGroup> similarNewsGroups = context.News
                            .Include(n => n.NewsGroups)
                            .ThenInclude(ng => ng.News)
                            .Where(ng => ninetyDaysAgo < ng.UpdatedDate)
                            .SelectMany(n => n.NewsGroups, (n, ng) => new {ID = n.ID, NewsGroup = ng})
                            .ToList()
                            .GroupBy(n => n.NewsGroup)
                            .Select(agg => new
                            {
                                NewsGroup = agg.Key,
                                SimilarityScore = agg.Average(n => similarity.Value[n.ID])
                            })
                            .Where(ng => ng.SimilarityScore > threshold)
                            .Select(ng => ng.NewsGroup)
                            .ToList();

                        if(similarNewsGroups.Any()) // the article will be added to existing groups
                        {
                            foreach(NewsGroup newsGroup in similarNewsGroups)
                            {
                                newsGroup.News.Add(news);
                            }
                        } else // a new group should be created
                        {
                            NewsGroup newsGroup = new NewsGroup();
                            newsGroup.Score = 0;
                            newsGroup.CreatedDate = DateTime.Now;
                            newsGroup.UpdatedDate = DateTime.Now;
                            newsGroup.News = new List<News>();

                            newsGroup.News.Add(news);

                            context.NewsGroups.Add(newsGroup);
                        }

                        news.AssignedToStory = true;

                        context.SaveChanges();
                    }
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