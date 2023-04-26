using AutoMapper;
using MeowoofStore.Models;
using MeowoofStore.ViewModels;

namespace MeowoofStore.Profiles
{
    public class RegisterViewModelProfile:Profile
    {
        public RegisterViewModelProfile()
        {
            CreateMap<RegisterViewModel, Member>();
            CreateMap<Member, RegisterViewModel>();
        }
    }
}
