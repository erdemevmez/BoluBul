using System.ComponentModel.DataAnnotations;

namespace BoluBul.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Bu alan zorunludur.")]
        [MaxLength(150)]
        [Display(Name = "Ad soyad")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bu alan zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bu alan zorunludur.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bu alan zorunludur.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmiyor.")]
        [Display(Name = "Şifre tekrar")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "İşletme sahibi olarak kaydol")]
        public bool RegisterAsBusinessOwner { get; set; }
    }
}
