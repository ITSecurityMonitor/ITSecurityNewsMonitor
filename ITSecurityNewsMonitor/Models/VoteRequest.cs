using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.Models
{
    public class VoteRequest
    {
        public int ID { get; set; }
        public string OwnerID { get; set; }
        public bool Completed { get; set; }

        public int NewsGroupId { get; set; }
        public NewsGroup NewsGroup { get; set; }
    }
}
