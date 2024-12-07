using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace API.Dto
{
    public class RegisterDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConformPassword { get; set; }
        public string Role { get; internal set; }
    }
}
