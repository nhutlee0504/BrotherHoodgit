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

        [ForeignKey("Product")]
        public int IDProduct { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }
    }
}
