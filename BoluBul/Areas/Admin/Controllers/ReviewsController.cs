using BoluBul.Controllers;
using BoluBul.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoluBul.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReviewsController : AppControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index(bool pendingOnly = false)
        {
            try
            {
                ViewBag.PendingOnly = pendingOnly;
                return View(await _reviewService.GetAdminReviewsAsync(pendingOnly));
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
            TempData["Success"] = await _reviewService.ApproveReviewAsync(id)
                ? "Yorum onaylandı."
                : "Yorum bulunamadı.";
            return RedirectToAction(nameof(Index), new { pendingOnly = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            TempData["Success"] = await _reviewService.DeleteReviewAsync(id)
                ? "Yorum silindi."
                : "Yorum bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
    }
}
