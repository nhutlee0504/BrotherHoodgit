
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface IBillDetail
    {
        public Task<IEnumerable<BillDetail>> GetBillDetails();
        public Task<IEnumerable<BillDetail>> GetBillDetailsByIDBill(int IDBill);
        public Task<BillDetail> AddBillDetail(BillDetail billDetail);
    }
}
