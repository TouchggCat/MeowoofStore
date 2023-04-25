using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MeowoofStore.Models.Validators;

namespace MeowoofStore.ViewModels
{
    public class LoginViewModel
    {
        [StringLength(255)]
        [Required(ErrorMessage ="請填寫您的信箱。")]
        [DisplayName("信箱帳號")]
        [EmailAddress(ErrorMessage = "請輸入正確的信箱格式")]
        [LoginAccountValidator]
        public string? Email { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "請填寫您的密碼。")]
        [DisplayName("密碼")]
        [LoginPasswordValidator]

        public string? Password { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
