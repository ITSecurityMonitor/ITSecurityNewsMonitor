using ITSecurityNewsMonitor.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.ViewModels
{
    public class UserRole
    {
        public IdentityUser User { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }

    public class AdminIndexViewModel
    {
        public List<UserRole> UserRoles { get; set; }
        public List<Source> Sources { get; set; }
        public List<News> NewsLeft { get; set; }
        public List<News> NewsRight { get; set; }
    }
}
