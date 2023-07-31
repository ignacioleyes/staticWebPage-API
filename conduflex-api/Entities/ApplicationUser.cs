using Microsoft.AspNetCore.Identity;

namespace conduflex_api.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public UserTypeEnum UserType { get; set; }
    }

    public enum UserTypeEnum
    {
        Admin
    }
}
