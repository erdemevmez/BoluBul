using BoluBul.Controllers;
using BoluBul.Data;
using BoluBul.Services.Interfaces;
using BoluBul.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoluBul.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : AppControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBusinessService _businessService;
        private readonly IReviewService _reviewService;

        public DashboardController(
            ApplicationDbContext context,
            IBusinessService businessService,
            IReviewService reviewService)
        {
            _context = context;
            _businessService = businessService;
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var model = new AdminDashboardViewModel
                {
                    TotalBusinesses = await _context.Businesses.CountAsync(),
                    PendingBusinesses = await _context.Businesses.CountAsync(b => !b.IsApproved),
                    TotalUsers = await _context.Users.CountAsync(),
                    TotalReviews = await _context.Reviews.CountAsync(),
                    TotalCategories = await _context.Categories.CountAsync(),
                    LatestBusinesses = (await _businessService.GetAdminBusinessesAsync()).Take(5).ToList(),
                    LatestReviews = (await _reviewService.GetAdminReviewsAsync()).Take(5).ToList()
                };

                return View(model);
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }
        }
    }
}
