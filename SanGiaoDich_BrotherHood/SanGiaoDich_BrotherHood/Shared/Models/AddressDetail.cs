using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SanGiaoDich_BrotherHood.Shared.Models
{
    public class AddressDetail
    {
        [Key]
        public int IDAddress { get; set; }

        [Required(ErrorMessage = "Vui nhập Tỉnh/Thành phố"), Column(TypeName = "Nvarchar(50)")]
        public string ProvinceCity { get; set; }

        [Required(ErrorMessage = "Vui nhập Quận/Huyện"), Column(TypeName = "Nvarchar(50)")]
        public string District {  get; set; }

        [Required(ErrorMessage = "Vui nhập Phường/Xã"), Column(TypeName = "Nvarchar(50)")]
        public string Wardcommune { get; set; }

        [Required(ErrorMessage = "Vui nhập địa chỉ cụ thể"), Column(TypeName = "Nvarchar(50)")]
        public string AdditionalDetail { get; set; }

        [ForeignKey("Account")]              
        public string UserName { get; set; }

        public Account Account { get; set; }
    }
}
