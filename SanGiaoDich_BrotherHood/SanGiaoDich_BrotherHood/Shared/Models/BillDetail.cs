using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SanGiaoDich_BrotherHood.Shared.Models
{
    public class BillDetail
    {
        [Key]
        public int IDBillDetail { get; set; }

        [ForeignKey("Bill")]
        public int IDBill { get; set; }

        [ForeignKey("Product")]
        public int IDProduct { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedDate { get; set; }
        public Bill Bill { get; set; }
        public Product Product { get; set; }
        public Rating Rating { get; set; }
    }
}
