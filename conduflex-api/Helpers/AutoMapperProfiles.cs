using AutoMapper;
using conduflex_api.DTOs;
using conduflex_api.Entities;
using System.Net.Sockets;

namespace conduflex_api.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles() 
        {
            CreateMap<ApplicationUser, LoginDTO>();
            CreateMap<ApplicationUser, ApplicationUserDTO>();
            CreateMap<ApplicationUserCreationDTO, ApplicationUser>();

            CreateMap<Branch, BranchDTO>();

            CreateMap<Product, ProductDTO>();
            CreateMap<ProductCreationDTO, Product>();
            CreateMap<ProductPatchDTO, Product>().ReverseMap();

            CreateMap<Contact, ContactDTO>();
            CreateMap<ContactCreationDTO, Contact>()
                .ForMember(c => c.CreationDate, c => c.MapFrom(dto => DateTime.SpecifyKind(dto.CreationDate, DateTimeKind.Utc)));

            CreateMap<Home, HomeDTO>();
            CreateMap<HomeCreationDTO, Home>();
            CreateMap<HomePatchDTO, Home>().ReverseMap();
        }
    }
}
