using System.ComponentModel.DataAnnotations;

namespace MeowoofStore.Models.Validators
{
    public class NumberMaxValidator : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            int intValue;
            if (int.TryParse(value.ToString(), out intValue) && intValue > 0&&intValue<10000)
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage ?? "請小於10000");
        }


    }
}
