using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SanGiaoDich_BrotherHood.Shared.Models
{
    public class ImageProduct
    {
        [Key]
        public int IDImage { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string Image { get; set; }

        [ForeignKey("Product")]
        public int IDProduct { get; set; }

        public Product Product { get; set; }
    }
}
