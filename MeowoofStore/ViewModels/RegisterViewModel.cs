using MeowoofStore.Models.Validators;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MeowoofStore.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "請填寫您的信箱。")]
        [DisplayName("信箱帳號")]
        [EmailAddress(ErrorMessage = "請輸入正確的信箱格式")]
        [RegisterValidator]
        public string? Email { get; set; }


        [Required(ErrorMessage = "請填寫您的密碼。")]
        [DisplayName("密碼")]
        public string? Password { get; set; }


        [Required(ErrorMessage = "請填寫會員名稱。")]
        [DisplayName("會員名稱")]
        public string? MemberName { get; set; }


        [Required(ErrorMessage = "請填寫您的住址。")]
        [DisplayName("住址")]
        public string? Address { get; set; }
    }
}
