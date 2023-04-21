﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MeowoofStore.ViewModels
{
    public class LoginViewModel
    {
        [StringLength(255)]
        [Required]
        [DisplayName("信箱帳號")]
        [EmailAddress(ErrorMessage = "請輸入正確的信箱格式")]
        public string? Email { get; set; }

        [StringLength(255)]
        [Required]
        [DisplayName("密碼")]
        public string? Password { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
