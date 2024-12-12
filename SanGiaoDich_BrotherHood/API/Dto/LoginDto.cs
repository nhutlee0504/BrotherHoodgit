using System.ComponentModel.DataAnnotations;

namespace API.Dto
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Tên tài khoản không được bỏ trống.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; }
    }
}
