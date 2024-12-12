
using Microsoft.EntityFrameworkCore;
using SanGiaoDich_BrotherHood.Server.Data;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class AddressDetailResponse : IAddressDetail
    {
        private readonly ApplicationDbContext _context;
        public AddressDetailResponse(ApplicationDbContext context) => _context = context;
        public async Task<AddressDetail> AddAddress(AddressDetail address)
        {
            try
            {
                await _context.AddressDetails.AddAsync(address);
                await _context.SaveChangesAsync();
                return address;
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        public async Task<AddressDetail> DeleteAddress(int IDAddress)
        {
            try
            {
                var address = await _context.AddressDetails.FirstOrDefaultAsync(x => x.IDAddress == IDAddress);
                if (address == null)
                {
                    return null;
                }
                _context.AddressDetails.Remove(address);
                _context.SaveChanges();
                return address;
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        public async Task<IEnumerable<AddressDetail>> GetAddressDetails()
        {
            return await _context.AddressDetails.ToListAsync();
        }

        public async Task<AddressDetail> GetAddressDetailsByIDAddress(int IDAddress)
        {
            try
            {
                var address = await _context.AddressDetails.FirstOrDefaultAsync(x => x.IDAddress == IDAddress);
                if(address == null)
                    return null;    
                return address;
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        public async Task<IEnumerable<AddressDetail>> GetAddressDetailsByUserName(string userName)
        {
            try
            {
                return await _context.AddressDetails.Where(x => x.UserName == userName).ToListAsync();
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        public async Task<AddressDetail> UpdateAddress(int IDAddress, AddressDetail address)
        {
            try
            {
                var addr = await _context.AddressDetails.FirstOrDefaultAsync(y => y.IDAddress == IDAddress);
                addr.ProvinceCity = address.ProvinceCity;
                addr.District = address.District;
                addr.Wardcommune = address.Wardcommune;
                addr.AdditionalDetail = address.AdditionalDetail;
                addr.UserName = address.UserName;
                _context.AddressDetails.Update(addr);   
                await _context.SaveChangesAsync();
                return addr;
            }
            catch (System.Exception)
            {

                return null;
            }
        }
    }
}
