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
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
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
        public async Task<IActionResult> Edit(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var existingUser = await _userManager.FindByIdAsync(user.Id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Актуализирайте полетата на потребителя
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.IdentificationNumber = user.IdentificationNumber;
            existingUser.Email = user.Email;

            var result = await _userManager.UpdateAsync(existingUser);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View(new User());
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
