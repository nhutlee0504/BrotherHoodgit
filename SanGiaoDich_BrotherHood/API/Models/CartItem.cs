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
        [ForeignKey("Cart")]
        public int IDCart { get; set; }
        [ForeignKey("Product")]
        public int IDProduct { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
