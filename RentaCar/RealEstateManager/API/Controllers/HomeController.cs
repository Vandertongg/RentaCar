using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentaCar.RealEstateManager.Database.Data;
using RentaCar.RealEstateManager.Database.Data.Entities;
using RentaCar.RealEstateManager.WebApp.Models;

namespace RentaCar.RealEstateManager.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly RentaCarDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;

        public HomeController(
            RentaCarDbContext context,
            ILogger<HomeController> logger,
            UserManager<User> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            try
            {
                var cars = _context.Cars.ToList();

                _logger.LogInformation("Successfully retrieved data for Home Index view.");
                return View(cars);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching data for Home Index.");
                return View("Error"); // Redirect to an error page if needed
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult UpdateAge()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAge(uint age)
        {
            if (age < 18)
            {
                ModelState.AddModelError("Age", "You must be at least 18 years old to rent a car.");
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            user.Age = age;
            await _userManager.UpdateAsync(user);

            // Запазете URL-а, от който потребителят е дошъл, за да го върнете обратно
            var returnUrl = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.Action("Index", "Home");
            }

            return Redirect(returnUrl);
        }
        public IActionResult SergeyKapaka()
        {
            return View();
        }

        public IActionResult DeyanGaykata()
        {
            return View();
        }

        public IActionResult MohammedHrachkomar()
        {
            return View();
        }

    }
}
