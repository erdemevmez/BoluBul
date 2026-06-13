using System.ComponentModel.DataAnnotations;

namespace BoluBul.Models
{
    public class District
    {
        public int Id { get; set; }

        public int CityId { get; set; }

        public City City { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string Slug { get; set; } = string.Empty;

        public ICollection<Neighborhood> Neighborhoods { get; set; } = new List<Neighborhood>();

        public ICollection<Business> Businesses { get; set; } = new List<Business>();
    }
}
