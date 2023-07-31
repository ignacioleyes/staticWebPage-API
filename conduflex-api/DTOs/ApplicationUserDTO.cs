using conduflex_api.Entities;

namespace conduflex_api.DTOs
{
    public class ApplicationUserDTO
    {
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public UserTypeEnum UserType { get; set; }
    }
}
