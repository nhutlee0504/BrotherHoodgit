using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SanGiaoDich_BrotherHood.Shared.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDProduct { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm"), Column(TypeName = "nvarchar(250)")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng sản phẩm")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm")]
        public decimal Price { get; set; }

        [ForeignKey("Category")]
        public int IDCategory { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Status { get; set; }

        [ForeignKey("Account")]
        public string UserName { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }
        public string? ProrityLevel { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string AccountAccept {  get; set; }
        public string? Reason { get; set; }
        public decimal PriceUp { get; set; }

        [JsonIgnore] 
        public Account Account { get; set; }
        [JsonIgnore]
        public ICollection<Favorite> favorites { get; set; }

        [JsonIgnore]
        public ICollection<BillDetail> billDetails { get; set; }

        [JsonIgnore]
        public ICollection<ImageProduct> imageProducts { get; set; }

        [JsonIgnore]
        public ICollection<CartItem> cartItem { get; set; }

        [JsonIgnore]
        public Category Category { get; set; }
    }
}
