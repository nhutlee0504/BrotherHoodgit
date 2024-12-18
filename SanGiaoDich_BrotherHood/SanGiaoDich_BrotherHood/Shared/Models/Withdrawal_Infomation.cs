using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace SanGiaoDich_BrotherHood.Shared.Models
{
    public class Withdrawal_Infomation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentReqID { get; set; }
        public string PaymentType { get; set; }
        public string AccountNumber { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Bank { get; set; }
        public string Status { get; set; }
        public string FullName { get; set; }
        [ForeignKey("Account")]
        public string UserName { get; set; }
        [JsonIgnore]
        public Account Account { get; set; }
    }
}
