using System.ComponentModel.DataAnnotations;

namespace conduflex_api.DTOs
{
    public class HomeCreationDTO
    {
        [Required]
        public string[] Images { get; set; }
        [Required]
        public string Title { get; set; }
        public string[] Description { get; set; }
    }
}
