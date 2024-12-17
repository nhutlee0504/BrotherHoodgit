using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Shared.Models
{
    public class PaymentRequestModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentReqID { get; set; }
        public string PaymentType { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TxnRef { get; set; }
        [ForeignKey("Account")]
        public string UserName { get; set; }
        [JsonIgnore]
        public Account Account { get; set; }
        public PaymentResponseModel PaymentResponse { get; set; }
    }

}

