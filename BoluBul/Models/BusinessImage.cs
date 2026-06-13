using System.ComponentModel.DataAnnotations;

namespace BoluBul.Models
{
    public class BusinessImage
    {
        public int Id { get; set; }

        public int BusinessId { get; set; }

        public Business Business { get; set; } = null!;

        [Required]
        [MaxLength(300)]
        public string ImageUrl { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? AltText { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
