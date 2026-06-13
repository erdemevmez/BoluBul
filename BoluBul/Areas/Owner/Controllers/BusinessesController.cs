using System.Security.Claims;
using BoluBul.Controllers;
using BoluBul.Services.Interfaces;
using BoluBul.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoluBul.Areas.Owner.Controllers
{
    [Area("Owner")]
    [Authorize(Roles = "BusinessOwner,Admin")]
    public class BusinessesController : AppControllerBase
    {
        private readonly IBusinessService _businessService;

        public BusinessesController(IBusinessService businessService)
        {
            _businessService = businessService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            try
            {
                return View(await _businessService.GetOwnerBusinessesAsync(userId, User.IsInRole("Admin")));
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(await _businessService.BuildCreateViewModelAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BusinessCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(await _businessService.BuildCreateViewModelAsync(model));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await _businessService.CreateBusinessAsync(model, userId, User.IsInRole("Admin"));
                TempData["Success"] = "İşletme başarıyla kaydedildi. Admin onayından sonra yayına alınır.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(await _businessService.BuildCreateViewModelAsync(model));
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _businessService.BuildEditViewModelAsync(
                id,
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                User.IsInRole("Admin"));

            return model is null ? Forbid() : View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BusinessEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var hydrated = await _businessService.BuildEditViewModelAsync(
                    model.Id,
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    User.IsInRole("Admin"));

                if (hydrated is null)
                {
                    return Forbid();
                }

                model.Categories = hydrated.Categories;
                model.Districts = hydrated.Districts;
                model.Neighborhoods = hydrated.Neighborhoods;
                model.ExistingLogoUrl = hydrated.ExistingLogoUrl;
                model.ExistingCoverImageUrl = hydrated.ExistingCoverImageUrl;

                return View(model);
            }

            try
            {
                var updated = await _businessService.UpdateBusinessAsync(
                    model,
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    User.IsInRole("Admin"));

                if (!updated)
                {
                    TempData["Error"] = "Bu işletmeye erişim yetkiniz yok.";
                    return Forbid();
                }

                TempData["Success"] = "İşletme bilgileri güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Images(int id)
        {
            var model = await _businessService.GetImageManageViewModelAsync(
                id,
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                User.IsInRole("Admin"));

            return model is null ? Forbid() : View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(int businessId, IFormFile? imageFile, string? altText)
        {
            try
            {
                var added = await _businessService.AddGalleryImageAsync(
                    businessId,
                    imageFile,
                    altText,
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    User.IsInRole("Admin"));

                if (!added)
                {
                    TempData["Error"] = "Fotoğraf yüklenemedi.";
                    return Forbid();
                }

                TempData["Success"] = "Fotoğraf başarıyla yüklendi.";
                return RedirectToAction(nameof(Images), new { id = businessId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["UploadError"] = ex.Message;
                return RedirectToAction(nameof(Images), new { id = businessId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int imageId, int businessId)
        {
            await _businessService.DeleteGalleryImageAsync(
                imageId,
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                User.IsInRole("Admin"));

            TempData["Success"] = "Fotoğraf silindi.";
            return RedirectToAction(nameof(Images), new { id = businessId });
        }

        [HttpGet]
        public async Task<IActionResult> Hours(int id)
        {
            var model = await _businessService.GetHoursViewModelAsync(
                id,
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                User.IsInRole("Admin"));

            return model is null ? Forbid() : View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Hours(BusinessHoursViewModel model)
        {
            var updated = await _businessService.UpdateHoursAsync(
                model,
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                User.IsInRole("Admin"));

            if (!updated)
            {
                TempData["Error"] = "Bu işletmeye erişim yetkiniz yok.";
                return Forbid();
            }

            TempData["Success"] = "Çalışma saatleri güncellendi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
