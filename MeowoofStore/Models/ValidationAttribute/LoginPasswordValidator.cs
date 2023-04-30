using MeowoofStore.Data;
using MeowoofStore.Models.Utilities;
using MeowoofStore.ViewModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;


namespace MeowoofStore.Models.Validators
{
    public class LoginPasswordValidator:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var _context = validationContext.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            var loginViewModel = (LoginViewModel)validationContext.ObjectInstance;
   

            if (!IsValidLogin(loginViewModel,_context))
            {
                return new ValidationResult("帳號或密碼錯誤");
            }

            return ValidationResult.Success;
        }

        private bool IsValidLogin(LoginViewModel loginViewModel,ApplicationDbContext  _context)
        {
            var member =  _context.Member.SingleOrDefault(m => m.Email == loginViewModel.Email);

            if (member == null)
                return false;

            var hashedPassword = member.Password;

            // Compute the hash of the user-provided password using the retrieved salt
            var hashPassword = PasswordAndSaltProcess
                                    .HashEnteredPassword( member.Salt, loginViewModel.Password);

            // Compare the computed hash with the stored hash
            if (hashPassword != hashedPassword)
            {
                return false;
            }

            return true;
        }
    }
}
