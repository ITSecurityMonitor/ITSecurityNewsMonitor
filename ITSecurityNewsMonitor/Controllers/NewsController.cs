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

        public async Task<IActionResult> Index(int? view, int page = 1, string search = "")
        {
            NewsIndexViewModel newsIndexViewModel = new NewsIndexViewModel();

            List<View> views = await _context.Views.Where(v => v.OwnerID.Equals(_userManager.GetUserId(User))).ToListAsync();
            List<NewsGroup> newsGroups = await _context.NewsGroups
                .Include(ng => ng.News).ThenInclude(n => n.Source)
                .Include(ng => ng.News).ThenInclude(n => n.LowLevelTags)
                .Include(ng => ng.VoteRequests)
                .Where(ng => !ng.Archived)
                .Where(ng => ng.News.Any(n => n.Headline.ToLower().Contains((search ?? "").ToLower())))
                .OrderByDescending(ng => ng.Score)
                .ThenByDescending(ng => ng.UpdatedDate)
                .ToListAsync();

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

        public async Task<IActionResult> Trackout(int newsGroupId, string link)
        {
            NewsGroup newsGroup = await _context.NewsGroups.Include(ng => ng.VoteRequests).Where(ng => ng.ID == newsGroupId).FirstOrDefaultAsync();
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

                _context.VoteRequests.Add(voteRequest);
                _context.SaveChangesAsync();
            }

            return Redirect(link);
        }

        public async Task<IActionResult> Details(int newsGroupId)
        {
            NewsDetailsViewModel newsDetailsViewModel = new NewsDetailsViewModel();
            NewsGroup newsGroup = await _context.NewsGroups
                .Where(ng => ng.ID == newsGroupId)
                .Include(ng => ng.News).ThenInclude(n => n.Source)
                .Include(ng => ng.News).ThenInclude(n => n.LowLevelTags)
                .Include(ng => ng.VoteRequests)
                .Where(ng => !ng.Archived)
                .FirstOrDefaultAsync();

            if (newsGroup == null)
            {
                return NotFound();
            }

            newsDetailsViewModel.NewsGroup = newsGroup;
            newsDetailsViewModel.OwnerId = _userManager.GetUserId(User);

            return View(newsDetailsViewModel);
        }

        public class UserVoteInput
        {
            public int newsGroupId { get; set; }
            public bool vote { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> UserVote([FromBody] UserVoteInput input)
        {
            NewsGroup newsGroup = await _context.NewsGroups.Include(ng => ng.VoteRequests).Where(ng => ng.ID == input.newsGroupId).FirstOrDefaultAsync();
            string ownerID = _userManager.GetUserId(User);

            if(newsGroup == null)
            {
                return NotFound("NewsGroup not found");
            }

            VoteRequest voteRequest = newsGroup.VoteRequests.Where(vr => vr.OwnerID == ownerID).FirstOrDefault();

            if(voteRequest == null)
            {
                return NotFound("VoteRequest not found");
            }

            Vote vote = new Vote();

            vote.OwnerID = ownerID;
            vote.Criticality = input.vote;
            vote.NewsGroup = newsGroup;

            _context.Add(vote);

            voteRequest.Completed = true;

            _context.SaveChangesAsync();

            return StatusCode(200);
        }
    }
}
