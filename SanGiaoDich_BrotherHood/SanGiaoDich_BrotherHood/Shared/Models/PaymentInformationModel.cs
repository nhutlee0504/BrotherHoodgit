using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Shared.Models
{
    public class PaymentInformationModel
    {
        public int PaymentID { get; set; }
        public string UserName { get; set; }
        public string PaymentType { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public DateTime CreatedDate { get; set; }


    }
}
