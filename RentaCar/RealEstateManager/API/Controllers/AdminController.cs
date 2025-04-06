using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentaCar.RealEstateManager.Database.Data.Entities;
using RentaCar.RealEstateManager.WebApp.Models;
using System.Threading.Tasks;

namespace RentaCar.RealEstateManager.API.Controllers
{
    [Authorize(Roles = "Admin")] // Само администраторите имат достъп
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        // GET: User
        public async Task<IActionResult> Index(string searchString)
        {
            // Запазване на текущата стойност на филтъра за използване в изгледа
            ViewData["CurrentFilter"] = searchString;

            // Извличане на всички потребители
            var users = _userManager.Users.AsQueryable();

            // Логика за филтриране
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.FirstName.Contains(searchString) ||
                                         u.LastName.Contains(searchString) ||
                                         u.Email.Contains(searchString) ||
                                         u.UserName.Contains(searchString));
            }

            // Конвертиране на резултата в списък
            var userList = await users.ToListAsync();

            return View(userList);
        }



        // GET: User/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user); // Връщаме директно обекта User към изгледа
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(string id, User updatedUser, IFormFile ProfilePicture)
        {
            // Намиране на потребителя по ID
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Актуализиране на профилната снимка
            if (ProfilePicture != null && ProfilePicture.Length > 0)
            {
                // Генериране на уникално име за файла
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePicture.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                // Запазване на файла в "wwwroot/images"
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePicture.CopyToAsync(stream);
                }

                // Запазване на пътя към файла в базата данни
                user.ProfilePicture = "/images/" + fileName;
            }

            // Актуализиране на другите полета
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Age = updatedUser.Age;
            user.IdentificationNumber = updatedUser.IdentificationNumber;

            // Запазване на промените в базата данни
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to update user.");
                return View(updatedUser);
            }

            return RedirectToAction("Index");
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user, string password)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            if (string.IsNullOrEmpty(user.UserName))
            {
                user.UserName = user.Email; // Задаваме потребителското име като имейл, ако не е зададено
            }

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User"); // Добавяме потребителя към роля "User"
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user);
        }

        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID = {userId} cannot be found.");
            }

            var roles = await _roleManager.Roles.ToListAsync();
            var model = new List<UserViewModel>();

            foreach (var role in roles)
            {
                model.Add(new UserViewModel
                {
                    RoleName = role.Name,
                    IsSelected = await _userManager.IsInRoleAsync(user, role.Name)
                });
            }

            ViewBag.UserId = userId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(string userId, List<UserViewModel> model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID = {userId} cannot be found.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Премахване на всички текущи роли, които не са избрани
            foreach (var role in currentRoles)
            {
                if (!model.Any(x => x.RoleName == role && x.IsSelected))
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }
            }

            // Добавяне на новоизбраните роли
            foreach (var role in model.Where(x => x.IsSelected).Select(x => x.RoleName))
            {
                if (!currentRoles.Contains(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
