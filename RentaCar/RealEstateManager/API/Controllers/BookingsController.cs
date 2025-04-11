using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentaCar.RealEstateManager.API.Services;
using RentaCar.RealEstateManager.Database.Data;
using RentaCar.RealEstateManager.Database.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace RentaCar.RealEstateManager.API.Controllers
{
    public class BookingsController : Controller
    {
        private readonly RentaCarDbContext _context;
        private readonly IBookingService _bookingService;


        public BookingsController(RentaCarDbContext context,IBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        // GET: Bookings
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string searchString, string sortByField, string sortOrder, int pageNumber = 1)
{
    // Задаване на текущите филтри
    ViewData["CurrentFilter"] = searchString;
    ViewData["CurrentSortField"] = sortByField;
    ViewData["CurrentSortOrder"] = sortOrder;

    // Започваме с основната заявка
    var bookingsQuery = _context.Bookings.Include(b => b.Car).Include(b => b.User).AsQueryable();

    // Търсене
    if (!string.IsNullOrEmpty(searchString))
    {
        bookingsQuery = bookingsQuery.Where(b =>
            b.User.UserName.Contains(searchString) ||
            b.Car.Model.Contains(searchString));
    }

    // Сортиране
    switch (sortByField)
    {
        case "User":
            bookingsQuery = sortOrder == "asc"
                ? bookingsQuery.OrderBy(b => b.User.UserName)
                : bookingsQuery.OrderByDescending(b => b.User.UserName);
            break;
        case "Car":
            bookingsQuery = sortOrder == "asc"
                ? bookingsQuery.OrderBy(b => b.Car.Model)
                : bookingsQuery.OrderByDescending(b => b.Car.Model);
            break;
        case "StartDate":
            bookingsQuery = sortOrder == "asc"
                ? bookingsQuery.OrderBy(b => b.StartDate)
                : bookingsQuery.OrderByDescending(b => b.StartDate);
            break;
        case "Status":
            bookingsQuery = sortOrder == "asc"
                ? bookingsQuery.OrderBy(b => b.Status)
                : bookingsQuery.OrderByDescending(b => b.Status);
            break;
        default:
            bookingsQuery = bookingsQuery.OrderBy(b => b.StartDate);
            break;
    }

    // Пагинация
    int pageSize = 10; // Брой записи на страница
    return View(await PaginatedList<Booking>.CreateAsync(bookingsQuery.AsNoTracking(), pageNumber, pageSize));
}


        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Pk == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewBag.Users = _context.Users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.UserName
            }).ToList();

            ViewBag.Cars = _context.Cars.Select(c => new SelectListItem
            {
                Value = c.Pk.ToString(),
                Text = c.Model
            }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Репопулиране на ViewBag при неуспешна валидация
            ViewBag.Users = _context.Users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.UserName
            }).ToList();

            ViewBag.Cars = _context.Cars.Select(c => new SelectListItem
            {
                Value = c.Pk.ToString(),
                Text = c.Model
            }).ToList();

            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["CarId"] = new SelectList(_context.Cars, "Pk", "Pk", booking.CarId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", booking.UserId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Booking booking)
        {
            var (isValid, message) = _bookingService.ValidateDates(booking.StartDate, booking.EndDate);

            if (!isValid)
            {
                ModelState.AddModelError("", message);

                ViewBag.Users = _context.Users.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.UserName }).ToList();
                ViewBag.Cars = _context.Cars.Select(c => new SelectListItem { Value = c.Pk.ToString(), Text = c.Model }).ToList();

                return View(booking);
            }

            if (ModelState.IsValid)
            {
                _context.Update(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = _context.Users.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.UserName }).ToList();
            ViewBag.Cars = _context.Cars.Select(c => new SelectListItem { Value = c.Pk.ToString(), Text = c.Model }).ToList();

            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Pk == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Pk == id);
        }
       
          
            

            [HttpPost]
            public async Task<IActionResult> Approve(int id)
            {
                var userRole = User.IsInRole("Admin") ? "Admin" : User.IsInRole("MehanicSpecialized") ? "MehanicSpecialized" : "";

                if (string.IsNullOrEmpty(userRole))
                {
                    return Forbid(); // Забрана за потребители без необходимата роля
                }

                var result = await _bookingService.ApproveBookingAsync(id, userRole);
                if (!result)
                {
                    return NotFound(); // Заявката не е намерена или неуспешно одобрение
                }

                return RedirectToAction(nameof(Index));
            }

            [HttpPost]
            public async Task<IActionResult> Reject(int id)
            {
                var userRole = User.IsInRole("Admin") ? "Admin" : User.IsInRole("MehanicSpecialized") ? "MehanicSpecialized" : "";

                if (string.IsNullOrEmpty(userRole))
                {
                    return Forbid(); // Забрана за потребители без необходимата роля
                }

                var result = await _bookingService.RejectBookingAsync(id, userRole);
                if (!result)
                {
                    return NotFound(); // Заявката не е намерена или неуспешно отказване
                }

                return RedirectToAction(nameof(Index));
            }
        [HttpPost]
        public async Task<IActionResult> ApproveBooking(int id)
        {
            var userRole = User.IsInRole("Admin") ? "Admin" :
                           User.IsInRole("MehanicSpecialized") ? "MehanicSpecialized" : "";

            if (string.IsNullOrEmpty(userRole))
            {
                return Forbid(); // Забрана за потребители без необходимата роля
            }

            var result = await _bookingService.ApproveBookingAsync(id, userRole);
            if (!result)
            {
                return NotFound(); // Заявката не е намерена или неуспешно одобрение
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RejectBooking(int id, string? rejectionReason)
        {
            var userRole = User.IsInRole("Admin") ? "Admin" :
                           User.IsInRole("MehanicSpecialized") ? "MehanicSpecialized" : "";

            if (string.IsNullOrEmpty(userRole))
            {
                return Forbid(); // Забрана за потребители без необходимата роля
            }

            var result = await _bookingService.RejectBookingAsync(id, userRole, rejectionReason);
            if (!result)
            {
                return NotFound(); // Заявката не е намерена или неуспешно отказване
            }

            return RedirectToAction(nameof(Index));
        }
    }

}