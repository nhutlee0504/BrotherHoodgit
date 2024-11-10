using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
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
        [ForeignKey("Product")]
        public int IDProduct { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
