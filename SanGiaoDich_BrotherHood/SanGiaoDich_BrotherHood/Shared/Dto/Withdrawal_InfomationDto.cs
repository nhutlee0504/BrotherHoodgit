using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Shared.Dto
{
    public class Withdrawal_InfomationDto
    {
        public string PaymentType { get; set; }
        public double Amount { get; set; }
        public string AccountNumber { get; set; }
        public string OrderDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Bank { get; set; }
        public string Status { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
    }
}
