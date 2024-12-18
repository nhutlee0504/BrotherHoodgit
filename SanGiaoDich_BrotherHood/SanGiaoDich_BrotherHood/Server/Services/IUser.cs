
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanGiaoDich_BrotherHood.Shared.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface IUser
    {
        public Task<IEnumerable<Account>> GetAllAccount();
        public Task<Account> RegisterUser(RegisterDto registerDto);
        public Task<string> LoginUser(LoginDto loginDto);
        public Task<Account> GetAccountInfo();
        public Task<Account> GetAccountByUserName(string userName);
		Task<Account> UpdateAccountInfo(string email);
        Task<Account> UpdateAccountInfo2(string phone, string description);
        Task<Account> UpdateProfileImage(IFormFile imageFile);
        public Task<Account> ChangePassword(string username, InfoAccountDto info);
        public Task<Account> AcceptIDCard(RecognitionDto recognitionDto);
        Task<Dictionary<string, int>> GetUserStatisticsAsync();
        // Thêm phương thức xuất file Excel cho thống kê người dùng
        Task<MemoryStream> ExportUserStatisticsToExcelAsync();
    }
}
