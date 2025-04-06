using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarsController(RentaCarDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Cars
        public async Task<IActionResult> Index(string searchString, string sortByField, string sortOrder, int? pageNumber)
        {
            // Сортиране
            ViewData["CurrentSort"] = sortByField;
            ViewData["CurrentSortOrder"] = sortOrder;

            // Филтриране
            var cars = from c in _context.Cars select c;

            if (!string.IsNullOrEmpty(searchString))
            {
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

            // Пагинация (10 записа на страница)
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

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        [HttpPost]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create(CarViewModel models)
        {
            if (ModelState.IsValid)
            {
                // Създаване на нов обект Car от CarViewModel
                var car = new Car
                {
                    Brand = models.Brand,
                    Model = models.Model,
                    Year = models.Year,
                    PricePerDay = models.PricePerDay,
                    PassangerSeats = models.PassangerSeats,
                    IsAvailable = models.IsAvailable,
                    Description = models.Description
                };

                // Проверка дали има качен файл
                if (models.Picture != null && models.Picture.Length > 0)
                {
                    // Път към папката wwwroot/uploads
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                    // Проверка дали папката съществува, ако не - създаване
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Генериране на уникално име за файла
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(models.Picture.FileName);

                    // Комбиниране на пътя и името на файла
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Записване на файла в папката wwwroot/uploads
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await models.Picture.CopyToAsync(fileStream);
                    }

                    // Запазване на относителния път към изображението в базата данни
                    car.Picture = $"/uploads/{uniqueFileName}";
                }

                // Добавяне на новия запис в базата данни
                _context.Cars.Add(car);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(models);
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

            // Създаване на ViewModel от модела Car
            var carViewModel = new CarViewModel
            {
                Pk = car.Pk,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                PricePerDay = car.PricePerDay,
                PassangerSeats = car.PassangerSeats,
                Description = car.Description,
                IsAvailable = car.IsAvailable,
                PicturePath = car.Picture // Пътят към съществуващото изображение
            };

            return View(carViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarViewModel carViewModel)
        {
            if (id != carViewModel.Pk)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var car = await _context.Cars.FindAsync(id);
                    if (car == null)
                    {
                        return NotFound();
                    }

                    // Актуализиране на полетата
                    car.Brand = carViewModel.Brand;
                    car.Model = carViewModel.Model;
                    car.Year = carViewModel.Year;
                    car.PricePerDay = carViewModel.PricePerDay;
                    car.PassangerSeats = carViewModel.PassangerSeats;
                    car.Description = carViewModel.Description;
                    car.IsAvailable = carViewModel.IsAvailable;

                    // Обработка на качената снимка
                    if (carViewModel.Picture != null && carViewModel.Picture.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(carViewModel.Picture.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await carViewModel.Picture.CopyToAsync(stream);
                        }

                        // Изтриване на старата снимка (ако има такава)
                        if (!string.IsNullOrEmpty(car.Picture))
                        {
                            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", car.Picture.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Запазване на новия път към файла
                        car.Picture = "/uploads/" + uniqueFileName;
                    }
                    else
                    {
                        // Ако няма качена нова снимка, оставяме текущата непроменена
                        // Не правим нищо с `car.Picture`
                    }

                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(carViewModel.Pk))
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

            return View(carViewModel);
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Pk == id);
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
        // GET: Cars/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car); // Това ще зареди Delete.cshtml с информацията за обекта
        }



        //Sort and filter

    }
}