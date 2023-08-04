using System.ComponentModel.DataAnnotations;

namespace conduflex_api.DTOs
{
    public class HomePatchDTO
    {
        [Required]
        public string[] Images { get; set; }
    }
}
