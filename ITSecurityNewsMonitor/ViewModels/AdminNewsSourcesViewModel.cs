using ITSecurityNewsMonitor.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.ViewModels
{
    public class AdminNewsSourcesViewModel
    {
        public List<Source> Sources { get; set; }
    }
}
