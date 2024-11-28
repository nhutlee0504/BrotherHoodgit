
using Microsoft.EntityFrameworkCore;
using SanGiaoDich_BrotherHood.Server.Data;
using SanGiaoDich_BrotherHood.Shared.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class BillDetailResponse : IBillDetail
    {
        private readonly ApplicationDbContext _context;
        public BillDetailResponse(ApplicationDbContext context) => _context = context;
        public async Task<BillDetail> AddBillDetail(BillDetailDto billDetail)
        {
            try
            {
                var newBd = new BillDetail
                {
                    IDBill = billDetail.IdBill,
                    IDProduct = billDetail.IdProduct,
                    Quantity = billDetail.Quantity,
                    Price = billDetail.Price,
                    CreatedDate = billDetail.CreatedDate,
                };
                await _context.BillDetails.AddAsync(newBd);
                await _context.SaveChangesAsync();
                return newBd;
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        public async Task<BillDetail> DeleteProductBillDetail(int IdBillDetail)
        {
            var BDFind = await _context.BillDetails.FindAsync(IdBillDetail);
            if (BDFind != null)
            {
                _context.BillDetails.Remove(BDFind);
                await _context.SaveChangesAsync();
            }
            throw new System.NotImplementedException();
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
