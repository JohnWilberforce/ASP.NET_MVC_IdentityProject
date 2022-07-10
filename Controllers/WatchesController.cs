using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityFromScratch.Models;
using Microsoft.AspNetCore.Authorization;

namespace IdentityFromScratch.Controllers
{
    public class WatchesController : Controller
    {
        private readonly MoviesContext _context;

        public WatchesController(MoviesContext context)
        {
            _context = context;
        }

        // GET: Watches
        [Authorize(Policy = "RequireUserManager")]
        public async Task<IActionResult> Index()
        {
              return _context.Watch != null ? 
                          View(await _context.Watch.ToListAsync()) :
                          Problem("Entity set 'MoviesContext.Watch'  is null.");
        }
        [Authorize(Policy = "RequireUserManager")]
        public async Task<IActionResult> UsersWatchedList()
        {

            return View();//the View corresponding to this is the View/Watches/UsersWatchedList
        }
        [HttpPost]
        [Authorize(Policy = "RequireUserManager")]
                public async Task<IActionResult> PostUsersWatchedList(string UserEmail)
        {
            //string res = Request.Form["UserEmail"];
            //this passes the View(..code..) below to the View/Watches/PostUsersWatchList
            return _context.Watch != null ?
                        View(await _context.Watch.Where(m=>m.UserEmail== UserEmail).ToListAsync()) :
                        Problem("Entity set 'MoviesContext.Watch'  is null.");
        }
        [Authorize]
        public async Task<IActionResult> IndividualUsersWatchedList()
        {

            return View();//the View corresponding to this is the View/Watches/UsersWatchedList
        }
        [HttpPost]
        public async Task<IActionResult> PostIndividualUsersWatchedList(string UserEmail)
        {
            //string res = Request.Form["UserEmail"];
            //this passes the View(..code..) below to the View/Watches/PostUsersWatchList
            if(UserEmail != User.Identity.Name)
            {
                return View("Denied");
            }
            return _context.Watch != null ?
                        View(await _context.Watch.Where(m => m.UserEmail == UserEmail).ToListAsync()) :
                        Problem("Entity set 'MoviesContext.Watch'  is null.");
        }


        // GET: Watches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Watch == null)
            {
                return NotFound();
            }

            var watch = await _context.Watch
                .FirstOrDefaultAsync(m => m.WatchId == id);
            if (watch == null)
            {
                return NotFound();
            }

            return View(watch);
        }

        // GET: Watches/Create
        [Authorize(Policy = "RequireUserManager")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Watches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireUserManager")]
        public async Task<IActionResult> Create([Bind("WatchId,Watched,UserEmail,MoviesId")] Watch watch)
        {
            if (ModelState.IsValid)
            {
                _context.Add(watch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(watch);
        }

        // GET: Watches/Edit/5
        [Authorize(Policy = "RequireUserManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Watch == null)
            {
                return NotFound();
            }

            var watch = await _context.Watch.FindAsync(id);
            if (watch == null)
            {
                return NotFound();
            }
            return View(watch);
        }

        // POST: Watches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireUserManager")]
        public async Task<IActionResult> Edit(int id, [Bind("WatchId,Watched,UserEmail,MoviesId")] Watch watch)
        {
            if (id != watch.WatchId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(watch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WatchExists(watch.WatchId))
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
            return View(watch);
        }

        // GET: Watches/Delete/5
        [Authorize(Policy = "RequireUserManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Watch == null)
            {
                return NotFound();
            }

            var watch = await _context.Watch
                .FirstOrDefaultAsync(m => m.WatchId == id);
            if (watch == null)
            {
                return NotFound();
            }

            return View(watch);
        }

        // POST: Watches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireUserManager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Watch == null)
            {
                return Problem("Entity set 'MoviesContext.Watch'  is null.");
            }
            var watch = await _context.Watch.FindAsync(id);
            if (watch != null)
            {
                _context.Watch.Remove(watch);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WatchExists(int id)
        {
          return (_context.Watch?.Any(e => e.WatchId == id)).GetValueOrDefault();
        }
    }
}
