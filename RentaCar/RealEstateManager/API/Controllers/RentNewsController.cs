using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentaCar.RealEstateManager.Database.Data;
using RentaCar.RealEstateManager.Database.Data.Entities;
using RentaCar.RealEstateManager.WebApp.Models;

namespace RentaCar.RealEstateManager.API.Controllers
{
    public class RentNewsController : Controller
    {
        private readonly RentaCarDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RentNewsController(RentaCarDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: RentNews
        public async Task<IActionResult> Index(string? title, string? content, DateTime? fromDate, DateTime? toDate, int pageNumber = 1, int pageSize = 10)
    {
        // Базова заявка към базата данни
        var query = _context.RentNews.AsQueryable();

        // Филтриране по заглавие
        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(n => n.Title != null && n.Title.Contains(title));
        }

        // Филтриране по съдържание
        if (!string.IsNullOrEmpty(content))
        {
            query = query.Where(n => n.Content != null && n.Content.Contains(content));
        }

        // Филтриране по дата (от-до)
        if (fromDate.HasValue)
        {
            query = query.Where(n => n.DatePosted >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(n => n.DatePosted <= toDate.Value);
        }

        // Пагинация
        var paginatedList = await PaginatedList<RentNews>.CreateAsync(query.OrderByDescending(n => n.DatePosted), pageNumber, pageSize);

        // Създаване на ViewModel с резултатите и филтрите
        var viewModel = new RentNewsFilterViewModel
        {
            Title = title,
            Content = content,
            FromDate = fromDate,
            ToDate = toDate,
            RentNews = paginatedList
        };

        return View(viewModel);
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
        public async Task<IActionResult> Create(RentNewsViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Създаване на нов обект RentNews от RentNewsViewModel
                var rentNews = new RentNews
                {
                    Title = model.Title,
                    Content = model.Content,
                    DatePosted = model.DatePosted
                };

                // Проверка дали има качен файл
                if (model.NewsPicture != null && model.NewsPicture.Length > 0)
                {
                    // Път към папката wwwroot/images
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                    // Проверка дали папката съществува, ако не - създаване
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Генериране на уникално име за файла
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.NewsPicture.FileName);

                    // Комбиниране на пътя и името на файла
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Записване на файла в папката wwwroot/images
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.NewsPicture.CopyToAsync(fileStream);
                    }

                    // Запазване на относителния път към изображението в базата данни
                    rentNews.NewsPicture = $"/uploads/{uniqueFileName}";
                }

                // Добавяне на новия запис в базата данни
                _context.RentNews.Add(rentNews);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
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
        public async Task<IActionResult> Edit(int id, RentNewsViewModel model)
        {
            if (id != model.Pk)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Намерете текущия запис в базата данни
                    var rentNews = await _context.RentNews.FindAsync(id);
                    if (rentNews == null)
                    {
                        return NotFound();
                    }

                    // Актуализиране на полетата
                    rentNews.Title = model.Title;
                    rentNews.Content = model.Content;
                    rentNews.DatePosted = model.DatePosted;

                    // Проверка дали има качен нов файл
                    if (model.NewsPicture != null && model.NewsPicture.Length > 0)
                    {
                        // Път към папката wwwroot/images
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                        // Проверка дали папката съществува, ако не - създаване
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Генериране на уникално име за файла
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.NewsPicture.FileName);

                        // Комбиниране на пътя и името на файла
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Записване на новия файл в папката wwwroot/images
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.NewsPicture.CopyToAsync(fileStream);
                        }

                        // Изтриване на старата снимка (ако има такава)
                        if (!string.IsNullOrEmpty(rentNews.NewsPicture))
                        {
                            string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, rentNews.NewsPicture.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Запазване на относителния път към новата снимка
                        rentNews.NewsPicture = $"/images/{uniqueFileName}";
                    }

                    _context.Update(rentNews);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentNewsExists(model.Pk))
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
            return View(model);
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
