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

        [DisplayName("數量")]
        public int Quantity { get; set; }


        [DisplayName("價格")]
        public int Price { get; set; }

        [DisplayName("總計")]
        public int TotalPrice { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
