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
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserManagement()
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

        public class DeleteBody
        {
            public string userId { get; set; }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser([FromBody] DeleteBody body)
        {
            IdentityUser user = await _userManager.FindByIdAsync(body.userId);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return StatusCode(200);
                else
                    return StatusCode(500);
            }
            else
                return NotFound();
        }

        public class ChangeRoleBody
        {
            public string userId { get; set; }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ChangeRole([FromBody] ChangeRoleBody body)
        {
            IdentityUser user = await _userManager.FindByIdAsync(body.userId);
            if (user != null)
            {
                IdentityResult result;

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, "Admin");
                }
                else
                {
                    result = await _userManager.AddToRoleAsync(user, "Admin");
                }

                if (result.Succeeded)
                    return StatusCode(200);
                else
                    return StatusCode(500);
            }
            else
                return NotFound();
        }
    }
}
