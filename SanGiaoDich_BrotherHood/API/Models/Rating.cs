using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Rating
    {
        [Key]
        public int IDRating { get; set; }

        [ForeignKey("Account")]
        public string UserName { get; set; }

        [ForeignKey("BillDetail")]
        public int IDBillDetail { get; set; }

        public int Star { get; set; }

        [Column(TypeName = "ntext")]
        public string Comment { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string Image { get; set; }

        public Account Account { get; set; }
        public BillDetail BillDetail { get; set; }
    }
}
