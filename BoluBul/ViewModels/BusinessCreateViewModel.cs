using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BoluBul.ViewModels
{
    public class BusinessCreateViewModel
    {
        [Required(ErrorMessage = "Bu alan zorunludur.")]
        [MinLength(2, ErrorMessage = "İşletme adı en az 2 karakter olmalıdır.")]
        [MaxLength(180, ErrorMessage = "İşletme adı en fazla 180 karakter olabilir.")]
        [Display(Name = "İşletme adı")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bu alan zorunludur.")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Bu alan zorunludur.")]
        [Display(Name = "İlçe")]
        public int DistrictId { get; set; }

        [Display(Name = "Mahalle")]
        public int? NeighborhoodId { get; set; }

        [Required(ErrorMessage = "Açıklama alanı boş bırakılamaz.")]
        [MaxLength(3000, ErrorMessage = "En fazla 3000 karakter yazabilirsiniz.")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [MaxLength(300, ErrorMessage = "En fazla 300 karakter yazabilirsiniz.")]
        [Display(Name = "Kısa açıklama")]
        public string? ShortDescription { get; set; }

        [MaxLength(30, ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }

        [MaxLength(30, ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "WhatsApp")]
        public string? WhatsApp { get; set; }

        [MaxLength(150, ErrorMessage = "En fazla 150 karakter yazabilirsiniz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string? Email { get; set; }

        [MaxLength(250, ErrorMessage = "En fazla 250 karakter yazabilirsiniz.")]
        [Url(ErrorMessage = "Geçerli bir web adresi giriniz.")]
        [Display(Name = "Web sitesi")]
        public string? WebsiteUrl { get; set; }

        [Required(ErrorMessage = "Adres alanı boş bırakılamaz.")]
        [MaxLength(500, ErrorMessage = "En fazla 500 karakter yazabilirsiniz.")]
        [Display(Name = "Adres")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Enlem")]
        public decimal? Latitude { get; set; }

        [Display(Name = "Boylam")]
        public decimal? Longitude { get; set; }

        [Display(Name = "Logo")]
        public IFormFile? LogoFile { get; set; }

        [Display(Name = "Kapak görseli")]
        public IFormFile? CoverFile { get; set; }

        public IReadOnlyList<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

        public IReadOnlyList<SelectListItem> Districts { get; set; } = new List<SelectListItem>();

        public IReadOnlyList<SelectListItem> Neighborhoods { get; set; } = new List<SelectListItem>();
    }
}
