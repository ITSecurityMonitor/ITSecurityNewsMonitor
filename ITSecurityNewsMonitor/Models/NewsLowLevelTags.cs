using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.Models
{
    public class NewsLowLevelTag
    {
        public int ID { get; set; }

        public List<News> News { get; set; }
    }
}
