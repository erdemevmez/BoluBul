using System.ComponentModel.DataAnnotations;

namespace BoluBul.Models
{
    public class Neighborhood
    {
        public int Id { get; set; }

        public int DistrictId { get; set; }

        public District District { get; set; } = null!;

        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(140)]
        public string Slug { get; set; } = string.Empty;

        public ICollection<Business> Businesses { get; set; } = new List<Business>();
    }
}
