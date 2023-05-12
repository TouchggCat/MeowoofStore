using AutoMapper;
using MeowoofStore.Dtos;
using MeowoofStore.Models;

namespace MeowoofStore.Profiles
{
    public class AddToCartDtoProfile:Profile
    {
        public AddToCartDtoProfile()
        {
            CreateMap<Product, AddToCartDto>();
            CreateMap<AddToCartDto, Product>();
        }
    }
}
