using System.ComponentModel.DataAnnotations;

namespace BoluBul.Models
{
    public class City
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string Slug { get; set; } = string.Empty;

        public int PlateCode { get; set; }

        public ICollection<District> Districts { get; set; } = new List<District>();

        public ICollection<Business> Businesses { get; set; } = new List<Business>();
    }
}
