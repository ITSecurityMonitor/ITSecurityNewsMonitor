using ITSecurityNewsMonitor.Data;
using ITSecurityNewsMonitor.Helper;
using ITSecurityNewsMonitor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        private string _url;

        IServiceProvider _serviceProvider;
        public Crawler(IServiceProvider serviceProvider, IConfiguration config)
        {
            _serviceProvider = serviceProvider;
            _config = config;

            _url = "http://" + _config.GetValue<string>("Connections:Crawler:IP") + ":" + _config.GetValue<string>("Connections:Crawler:Port");
        }

        public async Task ExecuteCrawl()
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            using (SecNewsDbContext context = scope.ServiceProvider.GetRequiredService<SecNewsDbContext>())
            {
                List<LowLevelTag> lowLevelTags = context.LowLevelTags.Include(llt => llt.Keywords).ToList();
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
                            news.Source = source;
                            news.ManuallyAssigned = false;

                            List<int> tags = await ExtractTags(news.Content, lowLevelTags);

                            news.LowLevelTags = context.LowLevelTags.Where(llt => tags.Contains(llt.ID)).ToList();

                            context.News.Add(news);

                            NewsGroup newsGroup = new NewsGroup();
                            newsGroup.Score = 0;
                            newsGroup.CreatedDate = DateTime.Now;
                            newsGroup.UpdatedDate = DateTime.Now;
                            newsGroup.News = new List<News>();

                            newsGroup.News.Add(news);

                            context.NewsGroups.Add(newsGroup);
                        }
                    }
                }

                context.SaveChanges();

                Dictionary<int, int> similarities = await ComputeSimilarities(context.News.ToList());

                foreach(KeyValuePair<int, int> similarity in similarities)
                {
                    News news = context.News.Where(n => n.ID == similarity.Key).Include(n => n.NewsGroup).ThenInclude(ng => ng.News).First();
                        
                    if(news.NewsGroup.News.Count() > 1) // don't assign a news to a group if it is already in a group with other news
                    {
                        continue;
                    }

                    NewsGroup oldNewsGroup = news.NewsGroup; // save old news group so that it can be deleted
                    NewsGroup newNewsGroup = context.News.Where(n => n.ID == similarity.Value).Include(n => n.NewsGroup).First().NewsGroup;
                    news.NewsGroup = newNewsGroup;
                    news.NewsGroup.UpdatedDate = newNewsGroup.News.Max(n => n.CreatedDate) > news.CreatedDate ? newNewsGroup.News.Max(n => n.CreatedDate) : news.CreatedDate;

                    context.NewsGroups.Remove(oldNewsGroup);
                }

                context.SaveChanges();
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

        public async Task<List<int>> ExtractTags(string text, List<LowLevelTag> lowLevelTags)
        {
            var input = System.Text.Json.JsonSerializer.Serialize(new
            {
                text = text,
                keywords = lowLevelTags.Select(llt => new
                {
                    name = llt.ID,
                    keywords = llt.Keywords.ToList()
                })
            }, _options);

            var content = new StringContent(input, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(_url + "/keywords", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                List<int> responseObject = System.Text.Json.JsonSerializer.Deserialize<List<int>>(responseContent);

                return responseObject;
            }
            else
            {
                throw new System.ArgumentException("API returned status code: " + response.StatusCode);
            }
        }

        public async Task<Dictionary<int, int>> ComputeSimilarities(List<News> news)
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
                Dictionary<int, int> responseObject = JsonConvert.DeserializeObject<Dictionary<int, int>>(responseContent);

                return responseObject;
            }
            else
            {
                throw new System.ArgumentException("API returned status code: " + response.StatusCode);
            }
        }
    }
}