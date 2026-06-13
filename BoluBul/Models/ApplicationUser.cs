using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BoluBul.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(150)]
        public string? FullName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(300)]
        public string? ProfileImageUrl { get; set; }

        public ICollection<Business> OwnedBusinesses { get; set; } = new List<Business>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
