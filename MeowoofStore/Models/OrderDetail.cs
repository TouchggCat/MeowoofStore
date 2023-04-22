using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MeowoofStore.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        [StringLength(255)]
        [DisplayName("訂單編號")]
        public Guid OrderNumber { get; set; }

        [DisplayName("總計")]
        public int TotalPrice { get; set; }

        public bool IsShopping { get; set; } 

        public int MemberId { get; set; }

        public int ProductId { get; set; }
        public Member? Member { get; set; }
        public Product? Product { get; set; }
    }
}
