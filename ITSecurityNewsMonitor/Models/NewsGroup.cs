using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.Models
{
    public class NewsGroup
    {
        public int ID { get; set; }
        public int Score { get; set; }

        public int LowLevelTagId { get; set; }
        public LowLevelTag LowLevelTag { get; set; }

        public News News { get; set; }
    }
}
