using System.Security.Claims;
using BoluBul.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoluBul.Controllers
{
    [AllowAnonymous]
    public class BusinessesController : AppControllerBase
    {
        private readonly IBusinessService _businessService;
        private readonly IBusinessStatService _businessStatService;

        public BusinessesController(IBusinessService businessService, IBusinessStatService businessStatService)
        {
            _businessService = businessService;
            _businessStatService = businessStatService;
        }

        public async Task<IActionResult> Index(string? search, string? category, int? districtId, string? sort)
        {
            try
            {
                var model = await _businessService.SearchBusinessesAsync(search, category, districtId, sort);
                return View(model);
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }
        }

        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return NotFound();
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var model = await _businessService.GetBusinessDetailBySlugAsync(slug, userId);

                if (model is null)
                {
                    return NotFound();
                }

                await _businessStatService.IncrementViewAsync(model.Id);
                return View(model);
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }
        }
    }
}
