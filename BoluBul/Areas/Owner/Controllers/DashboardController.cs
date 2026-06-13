using System.Security.Claims;
using BoluBul.Controllers;
using BoluBul.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoluBul.Areas.Owner.Controllers
{
    [Area("Owner")]
    [Authorize(Roles = "BusinessOwner,Admin")]
    public class DashboardController : AppControllerBase
    {
        private readonly IBusinessService _businessService;

        public DashboardController(IBusinessService businessService)
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
                var model = await _businessService.GetOwnerDashboardAsync(userId, User.IsInRole("Admin"));
                return View(model);
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }
        }
    }
}
