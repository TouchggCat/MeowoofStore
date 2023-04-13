using MeowoofStore.Models.Validators;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MeowoofStore.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("品名")]
        [StringLength(255)]
        public string? Name { get; set; }

        [DisplayName("產品描述")]
        public string? Description { get; set; }

        [NumberMaxValidator(ErrorMessage = "單價無法大於9999")] //邏輯不過時出現的訊息，會覆蓋NumberMaxValidator最後一行
        [DisplayName("單價")]
        [Required(ErrorMessage = "請輸入單價")] //修改沒輸入時預設訊息
        public int Price { get; set; }

        [NumberMaxValidator(ErrorMessage = "庫存無法大於9999")]
        [DisplayName("庫存")]
        [Required(ErrorMessage = "請輸入庫存")]
        public int Stock { get; set; }

        [StringLength(50)]
        [DisplayName("圖片")]
        public string? ImageString { get; set; }
    }
}
