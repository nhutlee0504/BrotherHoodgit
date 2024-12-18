
using SanGiaoDich_BrotherHood.Shared.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface IBill
    {
        public Task<IEnumerable<Bill>> GetBills();
        public Task<IEnumerable<Bill>> GetBillsByUserName(string userName);
        public Task<Bill> GetBillByIDBill(int IDBill);
        public Task<Bill> AddBill(BillDto bill);
        public Task<Bill> UpdateBill(int IDBill, Bill bill);
        public Task<Bill> AcceptBill(int IdBill);
        public Task<Bill> CancelBill(int IdBill);
        public Task<Bill> DoneBill(int IdBill, string status);
		public Task<object> GetOrderStatisticsAsync();
	}
}
