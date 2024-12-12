using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class BillDetailResponse : IBillDetail
    {
        private readonly ApplicationDbContext _context;
        public BillDetailResponse(ApplicationDbContext context) => _context = context;
        public async Task<BillDetail> AddBillDetail(BillDetail billDetail)
        {
            try
            {
                await _context.BillDetails.AddAsync(billDetail);
                await _context.SaveChangesAsync();
                return billDetail;
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        public async Task<IEnumerable<BillDetail>> GetBillDetails()
        {
            return await _context.BillDetails.ToListAsync();
        }

        public async Task<IEnumerable<BillDetail>> GetBillDetailsByIDBill(int IDBill)
        {
            return await _context.BillDetails.Where(x => x.IDBill == IDBill).ToListAsync();
        }
    }
}
