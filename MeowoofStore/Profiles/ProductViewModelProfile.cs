using AutoMapper;
using MeowoofStore.Dtos;
using MeowoofStore.Models;
using MeowoofStore.ViewModels;

namespace MeowoofStore.Profiles
{
    public class ProductViewModelProfile:Profile
    {
        public ProductViewModelProfile()
        {
            CreateMap<Product, ProductViewModel>();
            CreateMap<ProductViewModel, Product>();
        }
    }
}
