using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.Models
{
    public class Source
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }

        public List<News> News { get; set; }
    }
}
