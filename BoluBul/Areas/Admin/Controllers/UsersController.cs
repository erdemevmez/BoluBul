using BoluBul.Controllers;
using BoluBul.Models;
using BoluBul.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoluBul.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : AppControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _userManager.Users
                    .AsNoTracking()
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();

                var rows = new List<AdminUserRowViewModel>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    rows.Add(new AdminUserRowViewModel
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        CreatedAt = user.CreatedAt,
                        Roles = string.Join(", ", roles)
                    });
                }

                return View(rows);
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeBusinessOwner(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is not null && !await _userManager.IsInRoleAsync(user, "BusinessOwner"))
            {
                await _userManager.AddToRoleAsync(user, "BusinessOwner");
                TempData["Success"] = "Kullanıcı işletme sahibi rolüne eklendi.";
            }
            else
            {
                TempData["Info"] = "Kullanıcı zaten işletme sahibi olabilir.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
