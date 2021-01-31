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

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public List<News> News { get; set; }

        public List<VoteRequest> VoteRequests { get; set; }
    }
}
