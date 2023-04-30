using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MeowoofStore.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string? Token { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "請填寫您的密碼。")]
        [DisplayName("密碼")]
        public string? Password { get; set; }
    }
}
