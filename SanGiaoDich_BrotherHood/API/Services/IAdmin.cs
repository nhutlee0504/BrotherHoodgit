using API.Dto;
using API.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IAdmin
    {
        public Task<IEnumerable<Account>> GetAllAccount();
        public Task<Account> RegisterAdmin(RegisterDto registerDto);
        public Task<string> LoginAdmin(LoginDto loginDto);
        public Task<Account> BannedAccountHigh(string username, DateTime endDate);
        public Task<Account> BannedAccountLow(string username, DateTime endDate);
        public Task<Account> DeleteAccountHigh(string username);
        public Task<Account> DeleteAccountLow(string username);
        public Task<Product> CensorProduct(int idProd, string sensor);
        public Task<Rating> CensorRating(int idRating);
        public Task<Account> RegisterAccount(RegisterDto registerDto);
    }
}
