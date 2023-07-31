using AutoMapper;
using conduflex_api.DTOs;
using conduflex_api.Entities;

namespace conduflex_api.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles() 
        {
            CreateMap<ApplicationUser, LoginDTO>();
            CreateMap<ApplicationUser, ApplicationUserDTO>();
            CreateMap<ApplicationUserCreationDTO, ApplicationUser>();
        }
    }
}
