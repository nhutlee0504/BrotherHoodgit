using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Shared.Models
{
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemID { get; set; }

        [ForeignKey("Cart")]
        public int IDCart { get; set; }
        [JsonIgnore]
        public Cart Cart { get; set; }

        [ForeignKey("Product")] // Rõ ràng hóa quan hệ tới Product
        public int IDProduct { get; set; }
        [JsonIgnore]
        public Product Product { get; set; } // Đảm bảo chỉ có 1 quan hệ với Product
        public DateTime CreatedDate { get; set; }
    }

}
