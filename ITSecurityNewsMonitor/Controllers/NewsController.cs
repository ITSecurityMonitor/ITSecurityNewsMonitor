using ITSecurityNewsMonitor.Data;
using ITSecurityNewsMonitor.Models;
using ITSecurityNewsMonitor.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.Controllers
{
    public class NewsController : Controller
    {
        private readonly SecNewsDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NewsController(SecNewsDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            NewsIndexViewModel newsIndexViewModel = new NewsIndexViewModel();

            List<View> views = _context.Views.Where(v => v.OwnerID.Equals(_userManager.GetUserId(User))).ToList();
            List<NewsGroup> newsGroups = _context.NewsGroups.OrderByDescending(ng => ng.UpdatedDate).ToList();

            newsIndexViewModel.Views = views;
            newsIndexViewModel.NewsGroups = newsGroups;

            return View(newsIndexViewModel);
        }
    }
}
