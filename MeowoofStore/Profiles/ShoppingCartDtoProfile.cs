using AutoMapper;
using MeowoofStore.Dtos;
using MeowoofStore.Models;

namespace MeowoofStore.Profiles
{
    public class ShoppingCartDtoProfile:Profile
    {
        public ShoppingCartDtoProfile()
        {
            CreateMap<ShoppingCartDto, AddToCartDto>();
            CreateMap<AddToCartDto, ShoppingCartDto>();
        }
    }
}
