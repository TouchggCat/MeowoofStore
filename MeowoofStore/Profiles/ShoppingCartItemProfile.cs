using AutoMapper;
using MeowoofStore.Models;
using MeowoofStore.ViewModels;

namespace MeowoofStore.Profiles
{
    public class ShoppingCartItemProfile:Profile
    {
        public ShoppingCartItemProfile()
        {
            CreateMap<ShoppingCartItem, AddToCartViewModel>();
            CreateMap<AddToCartViewModel, ShoppingCartItem>()
                .ForMember(a => a.Id, b => b.MapFrom(c => c.id));
        }
    }
}
