using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class BillResponse : IBill
    {
        private readonly ApplicationDbContext _context;
        public BillResponse(ApplicationDbContext context) => _context = context;

        public async Task<Bill> AddBill(Bill bill)
        {
            try
            {
                await _context.Bills.AddAsync(bill);
                await _context.SaveChangesAsync();
                return bill;
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        public async Task<Bill> GetBillByIDBill(int IDBill)
        {
            try
            {
                var bill = await _context.Bills.FirstOrDefaultAsync(x => x.IDBill == IDBill);
                if (bill == null)
                    return null;
                return bill;
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        public async Task<IEnumerable<Bill>> GetBills()
        {
            return await _context.Bills.ToListAsync();
        }

        public async Task<IEnumerable<Bill>> GetBillsByUserName(string userName)
        {
            return await _context.Bills.Where(x => x.UserName == userName).ToListAsync();
        }

        public async Task<Bill> UpdateBill(int IDBill, Bill bill)
        {
            var billUpdate = await _context.Bills.FirstOrDefaultAsync(x => x.IDBill == IDBill);
            if (billUpdate == null)
                return null;
            billUpdate.Status = bill.Status;
            _context.Bills.Update(billUpdate);
            await _context.SaveChangesAsync();
            return billUpdate;
        }
    }
}
