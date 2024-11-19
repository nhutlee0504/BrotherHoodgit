using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class ImageProduct
    {
        [Key]
        public int IDImage { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string Image { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedDate { get; set; }

        public bool IsDelete { get; set; }

        [ForeignKey("Product")]
        public int IDProduct { get; set; }

        public Product Product { get; set; }
    }
}
