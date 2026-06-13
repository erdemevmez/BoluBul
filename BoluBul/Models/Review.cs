using System.ComponentModel.DataAnnotations;

namespace BoluBul.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int BusinessId { get; set; }

        public Business Business { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;

        public bool IsApproved { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
