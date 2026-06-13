using System.ComponentModel.DataAnnotations;

namespace BoluBul.Models
{
    public class Business
    {
        public int Id { get; set; }

        public string? OwnerId { get; set; }

        public ApplicationUser? Owner { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        public int CityId { get; set; }

        public City City { get; set; } = null!;

        public int DistrictId { get; set; }

        public District District { get; set; } = null!;

        public int? NeighborhoodId { get; set; }

        public Neighborhood? Neighborhood { get; set; }

        [Required]
        [MaxLength(180)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(220)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        [MaxLength(3000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? ShortDescription { get; set; }

        [MaxLength(30)]
        public string? Phone { get; set; }

        [MaxLength(30)]
        public string? WhatsApp { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(250)]
        public string? WebsiteUrl { get; set; }

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        [MaxLength(300)]
        public string? LogoUrl { get; set; }

        [MaxLength(300)]
        public string? CoverImageUrl { get; set; }

        public bool IsApproved { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsFeatured { get; set; }

        public decimal AverageRating { get; set; }

        public int ReviewCount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public ICollection<BusinessImage> Images { get; set; } = new List<BusinessImage>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public ICollection<BusinessHour> Hours { get; set; } = new List<BusinessHour>();

        public BusinessStat? Stats { get; set; }
    }
}
