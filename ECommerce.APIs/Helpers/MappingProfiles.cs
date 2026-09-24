using AutoMapper;
using ECommerce.APIs.DTOs;
using ECommerce.Core.Entities.Basket;
using ECommerce.Core.Entities.Identity;
using ECommerce.Core.Entities.Product;

namespace ECommerce.APIs.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.Brand, O => O.MapFrom(s => s.Brand.Name))
                .ForMember(d => d.Category, O => O.MapFrom(s => s.Category.Name))
                .ForMember(d => d.PictureUrl, O => O.MapFrom<ProductPictureUrlResolver>());

            CreateMap<CustomerBasketDto, CustomerBasket>();
            CreateMap<BasketItemDto, BasketItem>();

            CreateMap<Address, AddressDto>().ReverseMap();
            CreateMap<ECommerce.Core.Entities.Order_Aggregate.Address, AddressDto>().ReverseMap();
        }
    }
}


