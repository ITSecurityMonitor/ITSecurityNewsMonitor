using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.Models
{
    public class News
    {
        public int ID { get; set; }
        public string Headline { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public bool ManuallyAssigned { get; set; }

        public int NewsGroupId { get; set; }
        public NewsGroup NewsGroup { get; set; }

        public int SourceId { get; set; }
        public Source Source { get; set; }
    }
}
