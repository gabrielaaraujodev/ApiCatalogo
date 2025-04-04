using ApiCatalogo.Models;
using AutoMapper;

namespace ApiCatalogo.DTOs.AutomaticMapper
{
    public class ProductDtoMappingProfile : Profile
    {
        public ProductDtoMappingProfile()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Product,ProductDtoUpdateRequest>().ReverseMap();
            CreateMap<Product,ProductDtoUpdateResponse>().ReverseMap();
        }
    }
}
