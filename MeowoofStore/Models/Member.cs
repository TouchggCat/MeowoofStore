using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MeowoofStore.Models
{
    public class Member
    {
        public int Id { get; set; }

        [StringLength(255)]
        [Required]
        [DisplayName("信箱帳號")]
        [EmailAddress(ErrorMessage ="請輸入正確的信箱格式")]
        public string? Email { get; set; }

        [StringLength(255)]
        [Required]
        [DisplayName("密碼")]
        public string? Password { get; set; }

        [StringLength(255)]
        [Required]
        [DisplayName("會員名稱")]
        public string? MemberName { get; set; }

        [StringLength(255)]
        [Required]
        [DisplayName("住址")]
        public string? Address { get; set; }

        public Role? Role { get; set; }
        public byte RoleId { get; set; }

        public static readonly byte AdminRole = 1;

        public static readonly byte CustomerRole = 2;
    }
}
