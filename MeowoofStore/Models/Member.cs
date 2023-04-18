using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MeowoofStore.Models
{
    public class Member
    {
        public int id { get; set; }

        [StringLength(255)]
        [Required]
        [DisplayName("信箱帳號")]
        [EmailAddress(ErrorMessage ="請輸入正確的信箱格式")]
        public string? email { get; set; }

        [StringLength(255)]
        [Required]
        [DisplayName("密碼")]
        public string? password { get; set; }

        [StringLength(255)]
        [Required]
        [DisplayName("會員名稱")]
        public string? memberName { get; set; }

        public Role? Role { get; set; }
        public byte RoleId { get; set; }
    }
}
