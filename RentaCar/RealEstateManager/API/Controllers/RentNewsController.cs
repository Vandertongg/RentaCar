using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentaCar.RealEstateManager.Database.Data;
using RentaCar.RealEstateManager.Database.Data.Entities;

namespace RentaCar.RealEstateManager.API.Controllers
{
    public class RentNewsController : Controller
    {
        private readonly RentaCarDbContext _context;

        public RentNewsController(RentaCarDbContext context)
        {
            _context = context;
        }

        // GET: RentNews
        public async Task<IActionResult> Index()
        {
            return View(await _context.RentNews.ToListAsync());
        }

        // GET: RentNews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rentNews = await _context.RentNews
                .FirstOrDefaultAsync(m => m.Pk == id);
            if (rentNews == null)
            {
                return NotFound();
            }

            return View(rentNews);
        }

        // GET: RentNews/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RentNews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Pk,NewsPicture,Title,Content,DatePosted")] RentNews rentNews)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rentNews);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rentNews);
        }

        // GET: RentNews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rentNews = await _context.RentNews.FindAsync(id);
            if (rentNews == null)
            {
                return NotFound();
            }
            return View(rentNews);
        }

        // POST: RentNews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Pk,NewsPicture,Title,Content,DatePosted")] RentNews rentNews)
        {
            if (id != rentNews.Pk)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rentNews);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentNewsExists(rentNews.Pk))
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
            return View(rentNews);
        }

        // GET: RentNews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rentNews = await _context.RentNews
                .FirstOrDefaultAsync(m => m.Pk == id);
            if (rentNews == null)
            {
                return NotFound();
            }

            return View(rentNews);
        }

        // POST: RentNews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rentNews = await _context.RentNews.FindAsync(id);
            if (rentNews != null)
            {
                _context.RentNews.Remove(rentNews);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentNewsExists(int id)
        {
            return _context.RentNews.Any(e => e.Pk == id);
        }
    }
}
