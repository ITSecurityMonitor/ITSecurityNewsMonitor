using ITSecurityNewsMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.ViewModels
{
    public class NewsIndexViewModel
    {
        public List<View> Views { get; set; }
        public List<NewsGroup> NewsGroups { get; set; }
        public View SelectedView { get; set; }
        public int Page { get; set; }
        public int MaxPage { get; set; }
        public int NewsGroupCount { get; set; }
        public string Search { get; set; }
        public string OwnerId { get; set; }
    }
}
