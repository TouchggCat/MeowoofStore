using MeowoofStore.Models;
using System.ComponentModel;

namespace MeowoofStore.Dtos
{
    public class ShoppingCartDto
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public int Price { get; set; }

        public int TotalPrice => Quantity * Price;

        public Product? Product { get; set; }
    }
}
