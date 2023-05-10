using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MeowoofStore.Models
{
    public class Order
    {
        public int Id { get; set; }

        [StringLength(255)]
        [DisplayName("訂單編號")]
        public string? OrderNumber { get; set; }

        [StringLength(255)]
        [DisplayName("收件人姓名")]
        public string? ReceiverName { get; set; }

        [StringLength(255)]
        [DisplayName("收件人信箱")]
        public string? Email { get; set; }

        [StringLength(255)]
        [DisplayName("收件地址")]
        public string? Address { get; set; }

        [DisplayName("訂單日期")]
        public DateTime OrderDate { get; set; }

        [DisplayName("已寄送")]
        public bool IsShipping { get; set; }

        [DisplayName("已付款")]
        public bool IsPaid { get; set; }

        public int MemberId { get; set; }

        public Member? Member { get; set; }
    }
}
