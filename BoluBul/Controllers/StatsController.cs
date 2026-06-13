using BoluBul.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoluBul.Controllers
{
    [AllowAnonymous]
    public class StatsController : AppControllerBase
    {
        private readonly IBusinessService _businessService;
        private readonly IBusinessStatService _businessStatService;

        public StatsController(IBusinessService businessService, IBusinessStatService businessStatService)
        {
            _businessService = businessService;
            _businessStatService = businessStatService;
        }

        public async Task<IActionResult> PhoneClick(int businessId)
        {
            try
            {
                var business = await _businessService.GetBusinessForContactAsync(businessId);

                if (business is null || string.IsNullOrWhiteSpace(business.Phone))
                {
                    return NotFound();
                }

                await _businessStatService.IncrementPhoneClickAsync(businessId);
                return Redirect($"tel:{business.Phone}");
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }
        }

        public async Task<IActionResult> WhatsAppClick(int businessId)
        {
            try
            {
                var business = await _businessService.GetBusinessForContactAsync(businessId);

                if (business is null || string.IsNullOrWhiteSpace(business.WhatsApp))
                {
                    return NotFound();
                }

                await _businessStatService.IncrementWhatsAppClickAsync(businessId);
                var phone = business.WhatsApp.Replace("+", string.Empty).Replace(" ", string.Empty);

                return Redirect($"https://wa.me/{phone}");
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }
        }

        public async Task<IActionResult> DirectionClick(int businessId)
        {
            try
            {
                var business = await _businessService.GetBusinessForContactAsync(businessId);

                if (business is null)
                {
                    return NotFound();
                }

                await _businessStatService.IncrementDirectionClickAsync(businessId);

                var destination = business.Latitude.HasValue && business.Longitude.HasValue
                    ? $"{business.Latitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)},{business.Longitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}"
                    : Uri.EscapeDataString($"{business.Name} {business.Address} Bolu");

                return Redirect($"https://www.google.com/maps/dir/?api=1&destination={destination}");
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }
        }
    }
}
