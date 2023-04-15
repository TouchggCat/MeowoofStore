using AutoMapper;
using MeowoofStore.Models;
using MeowoofStore.ViewModels;

namespace MeowoofStore.Profiles
{
    public class AddToCartViewModelProfile:Profile
    {
        public AddToCartViewModelProfile()
        {
            CreateMap<Product, AddToCartViewModel>();
            CreateMap<AddToCartViewModel, Product>();
        }
    }
}
