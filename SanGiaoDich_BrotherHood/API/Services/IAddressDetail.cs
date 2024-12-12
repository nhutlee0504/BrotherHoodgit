using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IAddressDetail
    {
        public Task<IEnumerable<AddressDetail>> GetAddressDetails();
        public Task<IEnumerable<AddressDetail>> GetAddressDetailsByUserName(string userName);
        public Task<AddressDetail> GetAddressDetailsByIDAddress(int IDAddress);
        public Task<AddressDetail> AddAddress(AddressDetail address);
        public Task<AddressDetail> UpdateAddress(int IDAddress, AddressDetail address);
        public Task<AddressDetail> DeleteAddress(int IDAddress);
    }
}
