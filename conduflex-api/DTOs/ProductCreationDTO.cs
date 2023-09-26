using conduflex_api.Entities;
using System.ComponentModel.DataAnnotations;

namespace conduflex_api.DTOs
{
    public class ProductCreationDTO
    {
        [Required]
        public string Name { get; set; }
        public string EnglishName { get; set; }
        [Required]
        public string Description { get; set; }
        public string EnglishDescription { get; set; }
        public BrandEnum Brand { get; set; }
        [Required]
        public string Characteristics { get; set; }
        public string EnglishCharacteristics { get; set; }
        public float Price { get; set; }
        [Required]
        public string ProductImage { get; set; }
        public string CertificationsImage { get; set; }
        [Required]
        public string[] CharacteristicsImages { get; set; }
        [Required]
        public string TablesImage { get; set; }
        public string Application { get; set; }
        public string EnglishApplication { get; set; }
    }
}
