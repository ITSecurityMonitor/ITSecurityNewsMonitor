using ITSecurityNewsMonitor.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.ViewModels
{
    public class AdminSimilarityCheckViewModel
    { 
        public List<News> NewsLeft { get; set; }
        public List<News> NewsRight { get; set; }

        public News SelectionLeft { get; set; }
        public News SelectionRight { get; set; }
        public string JobID { get; set; }
    }
}
