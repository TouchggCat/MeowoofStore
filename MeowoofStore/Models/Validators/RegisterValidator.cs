using MeowoofStore.Data;
using System.ComponentModel.DataAnnotations;

namespace MeowoofStore.Models.Validators
{
    public class RegisterValidator:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var _context = validationContext.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;

            var member = validationContext.ObjectInstance as Member;
            var memberEmail = _context.Member.Where(n => n.Email == member.Email);
            if (memberEmail != null )
                return new ValidationResult("帳號已被使用");

            return ValidationResult.Success;

          
        }
    }
}
