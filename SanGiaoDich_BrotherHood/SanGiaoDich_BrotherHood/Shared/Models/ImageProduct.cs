using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SanGiaoDich_BrotherHood.Shared.Models
{
    public class ImageProduct
    {
        [Key]
        public int IDImage { get; set; }

        public string Image { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDelete {  get; set; }

        [ForeignKey("Product")]
        public int IDProduct { get; set; }

        public Product Product { get; set; }
    }
}
