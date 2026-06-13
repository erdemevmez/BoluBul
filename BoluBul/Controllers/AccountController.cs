using BoluBul.Models;
using BoluBul.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BoluBul.Controllers
{
    public class AccountController : AppControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
                    return View(model);
                }

                var user = await _userManager.FindByEmailAsync(model.Email);

                if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    TempData["Success"] = "Başarıyla giriş yaptınız.";
                    return LocalRedirect(model.ReturnUrl);
                }

                if (user is not null && await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    TempData["Success"] = "Başarıyla giriş yaptınız.";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }

                if (user is not null && await _userManager.IsInRoleAsync(user, "BusinessOwner"))
                {
                    TempData["Success"] = "Başarıyla giriş yaptınız.";
                    return RedirectToAction("Index", "Dashboard", new { area = "Owner" });
                }

                TempData["Success"] = "Başarıyla giriş yaptınız.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);
                }

                var role = model.RegisterAsBusinessOwner ? "BusinessOwner" : "User";
                await _userManager.AddToRoleAsync(user, role);
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["Success"] = "Hesabınız başarıyla oluşturuldu.";

                return model.RegisterAsBusinessOwner
                    ? RedirectToAction("Index", "Dashboard", new { area = "Owner" })
                    : RedirectToAction("Index", "Home");
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Info"] = "Çıkış yaptınız.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
