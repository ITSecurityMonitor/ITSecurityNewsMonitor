using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.Models
{
    public class HighLevelTag
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public List<LowLevelTag> LowLevelTags { get; set; }

        public ICollection<View> Views { get; set; }
    }
}
