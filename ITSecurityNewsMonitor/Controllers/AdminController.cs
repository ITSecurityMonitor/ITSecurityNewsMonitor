using Hangfire;
using ITSecurityNewsMonitor.Data;
using ITSecurityNewsMonitor.Models;
using ITSecurityNewsMonitor.Services;
using ITSecurityNewsMonitor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _cache;

        public AdminController(SecNewsDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IMemoryCache memoryCache)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _cache = memoryCache;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SimilarityCheck([FromQuery] string searchTermLeft, [FromQuery] string searchTermRight, [FromQuery] int? selectionLeft, [FromQuery] int? selectionRight)
        {

            AdminSimilarityCheckViewModel vm = new AdminSimilarityCheckViewModel();
            vm.NewsLeft = await _context.News.Where(n => searchTermLeft == null || n.Headline.Contains(searchTermLeft)).ToListAsync();
            vm.NewsRight = await _context.News.Where(n => searchTermRight == null || n.Headline.Contains(searchTermRight)).ToListAsync();
            vm.SelectionLeft = await _context.News.FindAsync(selectionLeft);
            vm.SelectionRight = await _context.News.FindAsync(selectionRight);

            if(vm.SelectionLeft != null && vm.SelectionRight != null)
            {
                string id = System.Guid.NewGuid().ToString();
                BackgroundJob.Enqueue<Crawler>(c => c.ComputeSimilarity(id, vm.SelectionLeft, vm.SelectionRight));
                vm.JobID = id;
            }

            ViewData["searchTermLeft"] = searchTermLeft;
            ViewData["searchTermRight"] = searchTermRight;
            
            return View(vm);
        }

        public class PollInput
        {
            public string id { get; set; }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PollSimilarity([FromBody] PollInput input)
        {
            string value = string.Empty;
            if (input.id != null && _cache.TryGetValue(input.id, out value))
            {
                return Content(value);
            } else
            {
                return StatusCode(102);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> NewsGroupAssignment()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> NewsSources()
        {
            AdminNewsSourcesViewModel vm = new AdminNewsSourcesViewModel();
            vm.Sources = await _context.Sources.ToListAsync();
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSource(int? ID)
        {
            try
            {
                if (ID == null)
                {
                    return NotFound();
                }

                Source source = await _context.Sources.FindAsync(ID);

                if(source == null)
                {
                    return NotFound();
                }

                _context.Sources.Remove(source);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(NewsSources));
            } catch(Exception e)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewsSources(string Name, string Link, string Homepage)
        {
            if(Name == null || Link == null || Homepage == null)
            {
                return StatusCode(500);
            }

            Source source = new Source()
            {
                Name = Name,
                Link = Link,
                Homepage = Homepage
            };

            try
            {
                if(_context.Sources.Where(s => s.Link.Equals(Link)).Any())
                {
                    return StatusCode(409);
                }

                _context.Sources.Add(source);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(NewsSources));
            } catch(Exception e)
            {
                return StatusCode(500);
            }
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
