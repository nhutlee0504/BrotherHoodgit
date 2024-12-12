using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SanGiaoDich_BrotherHood.Shared.Models
{
    public class Cart
    {
        [Key]
        public int IDCart { get; set; }

        [ForeignKey("Account")]
        public string UserName { get; set; }
        [JsonIgnore]
        public Account Account { get; set; }
        [JsonIgnore]
        public ICollection<CartItem> cartitem { get; set; }

    }
}
