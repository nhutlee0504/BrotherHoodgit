using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Bill
    {
        [Key]
        public int IDBill { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string FullName { get; set; }

        [Column(TypeName = "varchar(12)")]
        public string PhoneNumber { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string Email { get; set; }

        public string DeliveryAddress { get; set; }

        public decimal Total { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? DateReceipt { get; set; }

        [Column(TypeName = "nvarchar(70)")]
        public string PaymentType { get; set; }

        public string Status { get; set; }

        [ForeignKey("Account")]
        public string UserName { get; set; }

        public Account Account { get; set; }
        public ICollection<BillDetail> billDetails { get; set; }
    }
}
