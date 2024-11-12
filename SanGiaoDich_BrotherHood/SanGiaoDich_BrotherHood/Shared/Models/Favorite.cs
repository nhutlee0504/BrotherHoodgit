using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SanGiaoDich_BrotherHood.Shared.Models
{
    public class Favorite
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDFavorite { get; set; }

        [ForeignKey("Account")]
        public string UserName { get; set; }

        [ForeignKey("Product")]
        public int IDProduct {  get; set; }
        public DateTime CreatedDate { get; set; }

        public Account Account { get; set; }
        public Product Product { get; set; }
    }
}
