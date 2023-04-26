using MeowoofStore.Data;
using MeowoofStore.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace MeowoofStore.Models.Validators
{
    public class RegisterValidator:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var _context = validationContext.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;

            var viewModel = validationContext.ObjectInstance as RegisterViewModel;
            var memberEmail = _context.Member.Where(n => n.Email == viewModel.Email).SingleOrDefault();
            if (memberEmail != null )
                return new ValidationResult("帳號已被使用");

            return ValidationResult.Success;

          
        }
    }
}
