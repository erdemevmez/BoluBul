using BoluBul.Controllers;
using BoluBul.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoluBul.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BusinessesController : AppControllerBase
    {
        private readonly IBusinessService _businessService;

        public BusinessesController(IBusinessService businessService)
        {
            _businessService = businessService;
        }

        public async Task<IActionResult> Index(string? status)
        {
            try
            {
                ViewBag.Status = status;
                return View(await _businessService.GetAdminBusinessesAsync(status));
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            TempData["Success"] = await _businessService.ApproveBusinessAsync(id)
                ? "İşletme onaylandı."
                : "İşletme bulunamadı.";
            return RedirectToAction(nameof(Index), new { status = "pending" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFeatured(int id)
        {
            TempData["Success"] = await _businessService.ToggleFeaturedAsync(id)
                ? "İşletmenin öne çıkan durumu güncellendi."
                : "İşletme bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            TempData["Success"] = await _businessService.ToggleActiveAsync(id)
                ? "İşletmenin yayın durumu güncellendi."
                : "İşletme bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(string slug)
        {
            return RedirectToAction("Details", "Businesses", new { area = "", slug });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _businessService.BuildEditViewModelAsync(id, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, isAdmin: true);
            return model is null ? NotFound() : View("~/Areas/Owner/Views/Businesses/Edit.cshtml", model);
        }
    }
}
