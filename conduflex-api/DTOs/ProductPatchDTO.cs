using conduflex_api.Entities;

namespace conduflex_api.DTOs
{
    public class ProductPatchDTO
    {
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string Description { get; set; }
        public string EnglishDescription { get; set; }
        public BrandEnum Brand { get; set; }
        public string Characteristics { get; set; }
        public string EnglishCharacteristics { get; set; }
        public float Price { get; set; }
        public string ProductImage { get; set; }
        public string CertificationsImage { get; set; }
        public string[] CharacteristicsImages { get; set; }
        public string TablesImage { get; set; }
        public string Alternatives { get; set; }
        public string EnglishAlternatives { get; set; }
    }
}
