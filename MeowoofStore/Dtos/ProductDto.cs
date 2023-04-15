using MeowoofStore.Models.Validators;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MeowoofStore.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        [NumberMaxValidator(ErrorMessage = "單價無法大於9999")]
        public int Price { get; set; }

        [NumberMaxValidator(ErrorMessage = "庫存無法大於9999")]
        public int Stock { get; set; }

        public string? ImageString { get; set; }

    }
}
