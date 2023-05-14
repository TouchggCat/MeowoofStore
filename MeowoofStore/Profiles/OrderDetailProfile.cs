using AutoMapper;
using MeowoofStore.Models;
using MeowoofStore.ViewModels;

namespace MeowoofStore.Profiles
{
    public class OrderDetailProfile:Profile
    {
        public OrderDetailProfile()
        {
            CreateMap<OrderDetail, ShoppingCartViewModel>();
            CreateMap<ShoppingCartViewModel, OrderDetail>();
        }
    }
}
