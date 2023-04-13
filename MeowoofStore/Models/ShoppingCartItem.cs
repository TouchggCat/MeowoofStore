using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MeowoofStore.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        [DisplayName("數量")]
        public int Quantity { get; set; }

        [DisplayName("價格")]
        public int Price { get; set; }

        [DisplayName("小計")]
        public int TotalPrice => this.Quantity * this.Price;
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
