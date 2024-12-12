using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace SanGiaoDich_BrotherHood.Shared.Dto
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
        [StringLength(20, ErrorMessage = "Tên tài khoản tối đa 20 ký tự")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp")]
        public string ConformPassword { get; set; }
    }
}
