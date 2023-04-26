using MeowoofStore.Data;
using MeowoofStore.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace MeowoofStore.Models.Validators
{
    public class LoginAccountValidator : ValidationAttribute
    {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            ApplicationDbContext? _context =validationContext.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;

            var member = (LoginViewModel)validationContext.ObjectInstance;
            var email = _context.Member.Where(n => n.Email == member.Email).SingleOrDefault();
            if(email == null)
                return new ValidationResult("帳號或密碼錯誤");

            return ValidationResult.Success;
        }
    }
}
