using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.Models
{
    public class LowLevelTag
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public List<Keyword> Keywords { get; set; }

        public int HighLevelTagId { get; set; }
        public HighLevelTag HighLevelTag { get; set; }

        public ICollection<News> News { get; set; }
    }
}
