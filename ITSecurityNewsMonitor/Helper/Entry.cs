using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.Helper
{
    public class Entry
    {
        [JsonPropertyName("title")]
        public string Headline { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("summary_parsed")]
        public string Summary { get; set; }

        [JsonPropertyName("article_text")]
        public string Content { get; set; }

        [JsonPropertyName("published")]
        public DateTime Date { get; set; }

    }
}
