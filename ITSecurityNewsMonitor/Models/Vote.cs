using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.Models
{
    public class Vote
    {
        public int ID { get; set; }
        public string OwnerID { get; set; }
        public bool Criticality { get; set; }

        public int NewsGroupId { get; set; }
        public NewsGroup NewsGroup { get; set; }
    }
}
