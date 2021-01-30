using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.Models
{
    public class View
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public string OwnerID { get; set; }

        public ICollection<HighLevelTag> HighLevelTags { get; set; }
    }
}
