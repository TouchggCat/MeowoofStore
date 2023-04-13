using MeowoofStore.Models.Validators;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MeowoofStore.ViewModels
{
    public class AddToCartViewModel
    {
        public int id { get; set; }

        [DisplayName("產品名稱")]
        public string? Name { get; set; }

        [DisplayName("價格")]
        public int price { get; set; }

        [DisplayName("購買數量")]
        [NumberMaxValidator(ErrorMessage = "請輸入1到9999數量")]
        [Required(ErrorMessage = "請輸入數量")]
        public int count { get; set; }
    }
}
