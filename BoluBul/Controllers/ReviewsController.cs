using System.Security.Claims;
using BoluBul.Services.Interfaces;
using BoluBul.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoluBul.Controllers
{
    [Authorize]
    public class ReviewsController : AppControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Mine()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            try
            {
                return View(await _reviewService.GetUserReviewsAsync(userId));
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ReviewError"] = "Lütfen puan ve yorum alanlarını kontrol edin.";
                return RedirectToAction("Details", "Businesses", new { slug = model.BusinessSlug });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            try
            {
                await _reviewService.CreateReviewAsync(model, userId);
                TempData["ReviewSuccess"] = "Yorumunuz onay için gönderildi.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ReviewError"] = ex.Message;
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }

            return RedirectToAction("Details", "Businesses", new { slug = model.BusinessSlug });
        }
    }
}
