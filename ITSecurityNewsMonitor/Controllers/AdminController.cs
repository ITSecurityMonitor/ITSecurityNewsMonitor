using ITSecurityNewsMonitor.Data;
using ITSecurityNewsMonitor.Models;
using ITSecurityNewsMonitor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.Controllers
{
    public class AdminController : Controller
    {
        private readonly SecNewsDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(SecNewsDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            AdminIndexViewModel vm = new AdminIndexViewModel();

            vm.UserRoles = new List<UserRole>();

            List<IdentityUser> users = await _userManager.Users.ToListAsync();
            List<IdentityRole> roles = await _roleManager.Roles.ToListAsync();

            foreach(IdentityUser user in users)
            {
                UserRole userRole = new UserRole();
                userRole.User = user;
                userRole.Roles = new List<IdentityRole>();
                foreach (IdentityRole role in roles)
                {
                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        userRole.Roles.Add(role);
                    }
                }

                vm.UserRoles.Add(userRole);
            }
          
            return View(vm);
        }
    }
}
