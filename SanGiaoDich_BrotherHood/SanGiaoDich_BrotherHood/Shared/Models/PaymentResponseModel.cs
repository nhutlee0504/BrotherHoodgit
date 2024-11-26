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
    public class PaymentResponseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Khóa chính

        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public bool IsProcessed { get; set; }

        [ForeignKey("PaymentRequest")]
        public int PaymentReqID { get; set; }
        [JsonIgnore]
        public PaymentRequestModel PaymentRequest { get; set; }
    }
}
