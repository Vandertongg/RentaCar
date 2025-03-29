using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentaCar.RealEstateManager.Database.Data;
using RentaCar.RealEstateManager.Database.Data.Entities;
using RentaCar.RealEstateManager.WebApp.Models;

namespace RentaCar.RealEstateManager.API.Controllers
{
    public class CarsController : Controller
    {
        private readonly RentaCarDbContext _context;

        public CarsController(RentaCarDbContext context)
        {
            _context = context;
        }

        // GET: Cars
        public async Task<IActionResult> Index(string searchString, string sortByField, string sortOrder, int? pageNumber)
        {
            // Сортиране
            ViewData["CurrentSort"] = sortByField;
            ViewData["CurrentSortOrder"] = sortOrder;

            // Филтриране
            var cars = from c in _context.Cars select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                // Промяна за нечувствителност към главни/малки букви
                searchString = searchString.ToLower();
                cars = cars.Where(c => c.Brand.ToLower().Contains(searchString) || c.Model.ToLower().Contains(searchString));
            }

            // Прилагане на сортиране
            switch (sortByField)
            {
                case "Brand":
                    cars = sortOrder == "asc" ? cars.OrderBy(c => c.Brand) : cars.OrderByDescending(c => c.Brand);
                    break;
                case "Price":
                    cars = sortOrder == "asc" ? cars.OrderBy(c => c.PricePerDay) : cars.OrderByDescending(c => c.PricePerDay);
                    break;
                case "Year":
                    cars = sortOrder == "asc" ? cars.OrderBy(c => c.Year) : cars.OrderByDescending(c => c.Year);
                    break;
                default:
                    cars = cars.OrderBy(c => c.Brand);
                    break;
            }

            // Пагинация (пример с 10 записа на страница)
            int pageSize = 10;
            return View(await PaginatedList<Car>.CreateAsync(cars.AsNoTracking(), pageNumber ?? 1, pageSize));
        }



        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.Pk == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Pk,Brand,Model,Year,PricePerDay,PassangerSeats,Description,Picture,IsAvailable")] Car car)
        {
            if (ModelState.IsValid)
            {
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }


        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.Pk == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //Sort and filter

    }
}
