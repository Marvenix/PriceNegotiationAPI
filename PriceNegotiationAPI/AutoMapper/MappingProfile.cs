using AutoMapper;
using PriceNegotiationAPI.Model;
using PriceNegotiationAPI.Model.DTO;

namespace PriceNegotiationAPI.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>();
        }
    }
}
