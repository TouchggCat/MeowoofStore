using MeowoofStore.Models.Validators;
using System.ComponentModel;

namespace MeowoofStore.Dtos
{
    public class AddToCartDto
    {
        public int id { get; set; }

        public string? Name { get; set; }

        public int price { get; set; }

        public int Quantity { get; set; }
    }
}
