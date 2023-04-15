using AutoMapper;
using MeowoofStore.Dtos;
using MeowoofStore.Models;

namespace MeowoofStore.Profiles
{
    public class ProductProfile:Profile
    {
        public ProductProfile()
        {
        CreateMap<Product, ProductDto>();
        }
    }
}
