using System.ComponentModel.DataAnnotations;

namespace BoluBul.ViewModels
{
    public class ReviewCreateViewModel
    {
        [Required]
        public int BusinessId { get; set; }

        [Required]
        public string BusinessSlug { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Puan 1 ile 5 arasında olmalıdır.")]
        public int Rating { get; set; } = 5;

        [Required(ErrorMessage = "Yorum alanı boş bırakılamaz.")]
        [MaxLength(1000, ErrorMessage = "En fazla 1000 karakter yazabilirsiniz.")]
        public string Comment { get; set; } = string.Empty;
    }
}
