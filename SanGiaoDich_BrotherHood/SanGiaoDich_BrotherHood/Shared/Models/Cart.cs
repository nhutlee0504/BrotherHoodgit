using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SanGiaoDich_BrotherHood.Shared.Models
{
    public class Cart
    {
        [Key]
        public int IDCart { get; set; }

        [ForeignKey("Account"), Column(TypeName = "varchar(20)")]
        public string UserName { get; set; }

        [ForeignKey("Product")]
        public int IDProduct { get; set; }

        public int Quantity { get; set; }

        public Account Account { get; set; }
        public Product Product { get; set; }
    }
}
