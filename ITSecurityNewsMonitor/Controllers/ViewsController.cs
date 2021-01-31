using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ITSecurityNewsMonitor.Data;
using ITSecurityNewsMonitor.Models;

namespace ITSecurityNewsMonitor.Controllers
{
    public class ViewsController : Controller
    {
        private readonly SecNewsDbContext _context;

        public ViewsController(SecNewsDbContext context)
        {
            _context = context;
        }

        // GET: Views
        public async Task<IActionResult> Index()
        {
            return View(await _context.Views.ToListAsync());
        }

        // GET: Views/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var view = await _context.Views
                .FirstOrDefaultAsync(m => m.ID == id);
            if (view == null)
            {
                return NotFound();
            }

            return View(view);
        }

        // GET: Views/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Views/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,OwnerID")] View view)
        {
            if (ModelState.IsValid)
            {
                _context.Add(view);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(view);
        }

        // GET: Views/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var view = _context.Views.Include(v => v.HighLevelTags).Where(v => v.ID == id).First();
            if (view == null)
            {
                return NotFound();
            }

            ViewData["HighLevelTags"] = _context.HighLevelTags.ToList();
            return View(view);
        }

        // POST: Views/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,OwnerID")] View view)
        {
            if (id != view.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(view);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViewExists(view.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(view);
        }

        // GET: Views/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var view = await _context.Views
                .FirstOrDefaultAsync(m => m.ID == id);
            if (view == null)
            {
                return NotFound();
            }

            return View(view);
        }

        // POST: Views/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var view = await _context.Views.FindAsync(id);
            _context.Views.Remove(view);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ViewExists(int id)
        {
            return _context.Views.Any(e => e.ID == id);
        }
    }
}
