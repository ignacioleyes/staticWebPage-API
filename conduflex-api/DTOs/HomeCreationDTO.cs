using System.ComponentModel.DataAnnotations;

namespace conduflex_api.DTOs
{
    public class HomeCreationDTO
    {
        [Required]
        public string[] Images { get; set; }
    }
}
