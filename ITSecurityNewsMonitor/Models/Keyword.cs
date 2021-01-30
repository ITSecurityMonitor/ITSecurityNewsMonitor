using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.Models
{
    public class Keyword
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public int LowLevelTagId { get; set; }
        public LowLevelTag LowLevelTag { get; set; }
    }
}
