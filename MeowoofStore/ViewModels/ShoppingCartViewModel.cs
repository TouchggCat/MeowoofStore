using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MeowoofStore.Models;

namespace MeowoofStore.ViewModels
{
    public class ShoppingCartViewModel
    {
        public int Id { get; set; }

        [DisplayName("數量")]
        public int Quantity { get; set; }

        [DisplayName("價格")]
        public int Price { get; set; }

        [DisplayName("小計")]
        public int TotalPrice => Quantity * Price;
        public Product? Product { get; set; }
    }
}