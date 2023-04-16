using MeowoofStore.Models.Validators;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MeowoofStore.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [DisplayName("產品名稱")]
        [StringLength(255)]
        [Required(ErrorMessage = "請輸入正確的產品名稱")]
        public string? Name { get; set; }

        [DisplayName("產品描述")]
        public string? Description { get; set; }

        [NumberMaxValidator(ErrorMessage = "請輸入1~9999")] //邏輯不過時出現的訊息，會覆蓋NumberMaxValidator最後一行
        [DisplayName("單價")]
        [Required(ErrorMessage = "請輸入單價")] //修改沒輸入時預設訊息
        public int Price { get; set; }

        [NumberMaxValidator(ErrorMessage = "請輸入1~9999")]
        [DisplayName("庫存")]
        [Required(ErrorMessage = "請輸入庫存")]
        public int Stock { get; set; }

        [StringLength(50)]
        public string? ImageString { get; set; }

        public IFormFile? Photo { get; set; }
    }
}
