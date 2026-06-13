using Microsoft.AspNetCore.Http;

namespace BoluBul.ViewModels
{
    public class BusinessImageManageViewModel
    {
        public int BusinessId { get; set; }

        public string BusinessName { get; set; } = string.Empty;

        public IReadOnlyList<BusinessImageViewModel> Images { get; set; } = new List<BusinessImageViewModel>();

        public IFormFile? ImageFile { get; set; }

        public string? AltText { get; set; }
    }
}
