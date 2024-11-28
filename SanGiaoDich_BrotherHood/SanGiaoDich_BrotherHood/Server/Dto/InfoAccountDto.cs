using Microsoft.VisualBasic;
using System;

namespace SanGiaoDich_BrotherHood.Server.Dto
{
    public class InfoAccountDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone {  get; set; }
        public string Gender { get; set; }
        public string IdCard { get; set; }
        public DateTime? Birthday { get; set; }
        public string Introduce { get; set; }
        public string Password {  get; set; }
    }
}
