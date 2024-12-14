using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SanGiaoDich_BrotherHood.Shared.Models
{
    public class Account
    {
        [Key]
        public string UserName { get; set; } 

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu"), Column(TypeName = "varchar(100)"), MinLength(6, ErrorMessage = "Mật khẩu ít nhất 6 kí tự")]
        public string Password { get; set; } 

        [Column(TypeName = "nvarchar(150)")]
        public string FullName { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng"), Column(TypeName = "varchar(150)")]
        public string Email { get; set; } 

        [Column(TypeName = "varchar(12)")]
        public string PhoneNumber { get; set; }

        [Column(TypeName = "nvarchar(6)")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        public DateTime? Birthday { get; set; }

        [Required, Column(TypeName = "nvarchar(50)")]
        public string Role { get; set; }

        [Column(TypeName = "ntext")]
        public string Introduce { get; set; }
        public string ImageAccount { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime? TimeBanned { get; set; }

        public bool? IsDelete { get; set; }
        public decimal? PreSystem {  get; set; }
        public bool? IsActived { get; set; }
        public string ID { get; set; }
        public string Dob { get; set; }
        public string Home { get; set; }
        public string Nationality { get; set; }
        public string Doe { get; set; }


        public ICollection<Product> products { get; set; }
        public ICollection<Cart> carts { get; set; }
        public ICollection<Favorite> favorites { get; set; }
        public ICollection<Rating> ratings { get; set; }
        public ICollection<AddressDetail> addressDetails { get; set; }
        public ICollection<Bill> bills { get; set; }
        public ICollection<PaymentRequestModel> paymentRequests { get; set; }
        public ICollection<Conversation> conversations { get; set; }
       
    }
}
