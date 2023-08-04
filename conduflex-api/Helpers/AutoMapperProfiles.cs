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
            CreateMap<ContactCreationDTO, Contact>();
        }
    }
}
