using ITSecurityNewsMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.ViewModels
{
    public class ViewsIndexViewModel
    {
        public List<View> Views { get; set; }
        public string OwnerId { get; set; }
    }
}
