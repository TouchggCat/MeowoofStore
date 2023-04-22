using MeowoofStore.Models.Validators;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MeowoofStore.Models
{
    public class Member
    {
        public int Id { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "請填寫您的信箱。")]
        [DisplayName("信箱帳號")]
        [EmailAddress(ErrorMessage ="請輸入正確的信箱格式")]
        [RegisterValidator]
        public string? Email { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "請填寫您的密碼。")]
        [DisplayName("密碼")]
        public string? Password { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "請填寫會員名稱。")]
        [DisplayName("會員名稱")]
        public string? MemberName { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "請填寫您的住址。")]
        [DisplayName("住址")]
        public string? Address { get; set; }

        public Role? Role { get; set; }
        public byte RoleId { get; set; }

        public static readonly byte AdminRole = 1;

        public static readonly byte CustomerRole = 2;
    }
}
