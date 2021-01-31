using ITSecurityNewsMonitor.Data;
using ITSecurityNewsMonitor.Models;
using ITSecurityNewsMonitor.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Index(int? view, int page = 1, string search = "")
        {
            NewsIndexViewModel newsIndexViewModel = new NewsIndexViewModel();

            List<View> views = _context.Views.Where(v => v.OwnerID.Equals(_userManager.GetUserId(User))).ToList();
            List<NewsGroup> newsGroups = _context.NewsGroups
                .Include(ng => ng.News).ThenInclude(n => n.Source)
                .Include(ng => ng.News).ThenInclude(n => n.LowLevelTags)
                .Include(ng => ng.VoteRequests)
                .Where(ng => ng.News.Any(n => n.Headline.Contains(search ?? "")))
                .OrderByDescending(ng => ng.Score)
                .ThenByDescending(ng => ng.UpdatedDate)
                .ToList();

            double pageSize = 10.0;

            newsIndexViewModel.Views = views;
            newsIndexViewModel.MaxPage = newsGroups.Any() ? (int) Math.Ceiling(newsGroups.Count() / pageSize) : 1;
            newsIndexViewModel.OwnerId = _userManager.GetUserId(User);

            if (page > newsIndexViewModel.MaxPage)
            {
                page = newsIndexViewModel.MaxPage;
            }

            if(page < 1)
            {
                page = 1;
            }

            newsIndexViewModel.Page = page;

            List<View> selectedViews = views.Where(v => v.ID == view).ToList();

            if (view == null || !selectedViews.Any())
            {
                newsIndexViewModel.SelectedView = null;
            } else
            {
                newsIndexViewModel.SelectedView = selectedViews.First();
            }

            newsIndexViewModel.Search = search;

            newsIndexViewModel.NewsGroups = newsGroups.Skip((int)pageSize * (newsIndexViewModel.Page - 1)).Take((int)pageSize).ToList();

            return View(newsIndexViewModel);
        }

        public IActionResult Trackout(int newsGroupId, string link)
        {
            NewsGroup newsGroup = _context.NewsGroups.Find(newsGroupId);
            string ownerID = _userManager.GetUserId(User);

            if (newsGroup == null)
            {
                return NotFound();
            }

            if(!newsGroup.VoteRequests.Where(vr => vr.OwnerID.Equals(ownerID)).Any()) {
                VoteRequest voteRequest = new VoteRequest();

                voteRequest.Completed = false;
                voteRequest.OwnerID = ownerID;
                voteRequest.NewsGroup = newsGroup;

                _context.SaveChanges();
            }

            return Redirect(link);
        }
    }
}
