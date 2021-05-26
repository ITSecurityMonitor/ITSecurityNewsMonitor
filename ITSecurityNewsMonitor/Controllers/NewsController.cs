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

        public async Task<IActionResult> Index(int? view, int page = 1, string search = "", string order = "popular")
        {
            NewsIndexViewModel newsIndexViewModel = new NewsIndexViewModel();

            if(order == null || (!order.Equals("new") && !order.Equals("favorite")))
            {
                order = "popular";
            }

            ViewBag.order = order;

            List<View> views = await _context.Views.Where(v => v.OwnerID.Equals(_userManager.GetUserId(User))).ToListAsync();
            List<NewsGroup> newsGroups = await _context.NewsGroups
                .Include(ng => ng.News).ThenInclude(n => n.Source)
                .Include(ng => ng.News).ThenInclude(n => n.Tags)
                .Include(ng => ng.News).ThenInclude(n => n.LinkViewed)
                .Include(ng => ng.Favorites)
                .Where(ng => !ng.Archived)
                .Where(ng => ng.News.Any(n => n.Headline.ToLower().Contains((search ?? "").ToLower())))
                .ToListAsync();

            if(order.Equals("new"))
            {
                newsGroups = newsGroups.OrderByDescending(ng => ng.CreatedDate).ToList();
            } else
            {
                newsGroups = newsGroups.OrderByDescending(ng => ng.Score).ThenByDescending(ng => ng.News.Count()).ThenByDescending(ng => ng.UpdatedDate).ToList();
            }

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

        public async Task<IActionResult> Trackout(int newsId, string link)
        {
            News news = await _context.News.Include(n => n.LinkViewed).Where(n => n.ID == newsId).FirstOrDefaultAsync();
            string ownerID = _userManager.GetUserId(User);

            if (news == null)
            {
                return NotFound();
            }

            if(!news.LinkViewed.Any(lv => lv.OwnerID.Equals(ownerID))) {
                LinkViewed linkViewed = new LinkViewed();
                linkViewed.Date = DateTime.Now;
                linkViewed.OwnerID = ownerID;
                linkViewed.News = news;

                _context.LinksViewed.Add(linkViewed);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return StatusCode(500);
                }
            }

            return Redirect(link);
        }

        public class FavoriteInput
        {
            public int id { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Favorite([FromBody] FavoriteInput input)
        {
            NewsGroup newsGroup = await _context.NewsGroups.Include(ng => ng.Favorites).Where(ng => ng.ID == input.id).FirstOrDefaultAsync();

            if(newsGroup == null)
            {
                return NotFound();
            }

            string userId = _userManager.GetUserId(User);

            Favorite favorite = newsGroup.Favorites.Where(f => f.OwnerID.Equals(userId)).FirstOrDefault();

            if (favorite == null)
            {
                favorite = new Favorite();
                favorite.Date = DateTime.Now;
                favorite.OwnerID = userId;

                _context.Favorites.Add(favorite);

                newsGroup.Favorites.Add(favorite);
            } else 
            {
                _context.Favorites.Remove(favorite);
            }

            try
            {
                await _context.SaveChangesAsync();
                return StatusCode(200);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }

        public async Task<IActionResult> Details(int newsGroupId)
        {
            NewsDetailsViewModel newsDetailsViewModel = new NewsDetailsViewModel();
            NewsGroup newsGroup = await _context.NewsGroups
                .Where(ng => ng.ID == newsGroupId)
                .Include(ng => ng.News).ThenInclude(n => n.Source)
                .Include(ng => ng.News).ThenInclude(n => n.Tags)
                .Include(ng => ng.News).ThenInclude(n => n.LinkViewed)
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
    }
}
