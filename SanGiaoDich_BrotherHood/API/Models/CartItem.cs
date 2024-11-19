using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace API.Models
{
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemID { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("Cart")]
        public int IDCart { get; set; }

        public Cart Cart { get; set; }

        [ForeignKey("Product")] // Rõ ràng hóa quan hệ tới Product
        public int IDProduct { get; set; }

        public Product Product { get; set; } // Đảm bảo chỉ có 1 quan hệ với Product
    }


}
