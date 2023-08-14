using System.ComponentModel.DataAnnotations;

namespace conduflex_api.DTOs
{
    public class ContactCreationDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Message { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
