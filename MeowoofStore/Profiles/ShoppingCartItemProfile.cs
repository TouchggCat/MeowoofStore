using AutoMapper;
using MeowoofStore.ViewModels;

namespace MeowoofStore.Profiles
{
    public class ShoppingCartItemProfile:Profile
    {
        public ShoppingCartItemProfile()
        {
            CreateMap<ShoppingCartViewModel, AddToCartViewModel>();
            CreateMap<AddToCartViewModel, ShoppingCartViewModel>();
        }
    }
}
