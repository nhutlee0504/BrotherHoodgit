
using Microsoft.AspNetCore.Http;
using SanGiaoDich_BrotherHood.Server.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface IAdmin
    {
        public Task<IEnumerable<Account>> GetAllAccount();
        public Task<Account> RegisterAdmin(RegisterDto registerDto);
        public Task<string> LoginAdmin(LoginDto loginDto);
        public Task<Account> BannedAccount(string username);
        public Task<Account> UnBannedAccount (string username);
        public Task<Account> DeleteAccountHigh(string username);
        public Task<Account> DeleteAccountLow(string username);
        public Task<Product> CensorProduct(int idProd, string sensor);
        public Task<Rating> CensorRating(int idRating);
        public Task<Account> RegisterAccount(RegisterDto registerDto);
    }
}
