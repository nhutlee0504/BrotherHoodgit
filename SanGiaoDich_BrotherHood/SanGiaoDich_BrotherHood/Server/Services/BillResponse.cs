
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SanGiaoDich_BrotherHood.Server.Data;
using SanGiaoDich_BrotherHood.Shared.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class BillResponse : IBill
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BillResponse(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Bill> AddBill(BillDto bill)
        {
            var user = GetUserInfoFromClaims();
            var newBill = new Bill
            {
                FullName = bill.FullName,
                PhoneNumber = bill.PhoneNumber,
                Email = bill.Email,
                DeliveryAddress = bill.DeliveryAddress,
                Total = bill.Total,
                OrderDate = DateTime.Now,
                PaymentType = bill.PaymentType,
                Status = bill.Status,
                UserName = user.UserName
            };
            await _context.Bills.AddAsync(newBill);
            await _context.SaveChangesAsync();
            return newBill;
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
            return await _context.Bills.Where(x => x.UserName.Contains(userName)).ToListAsync();
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

        private (string UserName, string Email, string FullName, string PhoneNumber, string Gender, string IDCard, DateTime? Birthday, string ImageAccount, string Role, bool IsDelete, DateTime? TimeBanned) GetUserInfoFromClaims()
        {
            var userClaim = _httpContextAccessor.HttpContext?.User;
            if (userClaim != null && userClaim.Identity.IsAuthenticated)
            {
                // Kiểm tra thời gian hết hạn của token
                var expirationClaim = userClaim.FindFirst("exp");
                if (expirationClaim != null && long.TryParse(expirationClaim.Value, out long exp))
                {
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                    if (expirationTime < DateTime.UtcNow)
                    {
                        throw new UnauthorizedAccessException("Token đã hết hạn. Vui lòng đăng nhập lại.");
                    }
                }

                var userNameClaim = userClaim.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
                var emailClaim = userClaim.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
                var fullNameClaim = userClaim.FindFirst("FullName")?.Value ?? string.Empty;
                var phoneNumberClaim = userClaim.FindFirst("PhoneNumber")?.Value ?? string.Empty;
                var genderClaim = userClaim.FindFirst("Gender")?.Value ?? string.Empty;
                var idCardClaim = userClaim.FindFirst("IDCard")?.Value ?? string.Empty;
                var imageAccountClaim = userClaim.FindFirst("ImageAccount")?.Value ?? string.Empty;
                var roleClaim = userClaim.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

                DateTime? birthday = null;
                var birthdayClaimValue = userClaim.FindFirst("Birthday")?.Value;
                if (!string.IsNullOrWhiteSpace(birthdayClaimValue) && DateTime.TryParse(birthdayClaimValue, out DateTime parsedBirthday))
                {
                    birthday = parsedBirthday;
                }

                bool isDelete = false;
                var isDeleteClaimValue = userClaim.FindFirst("IsDelete")?.Value;
                if (isDeleteClaimValue != null && bool.TryParse(isDeleteClaimValue, out bool parsedIsDeleted))
                {
                    isDelete = parsedIsDeleted;
                }

                DateTime? timeBanned = null;
                var timeBannedClaimValue = userClaim.FindFirst("TimeBanned")?.Value;
                if (!string.IsNullOrWhiteSpace(timeBannedClaimValue) && DateTime.TryParse(timeBannedClaimValue, out DateTime parsedTimeBanned))
                {
                    timeBanned = parsedTimeBanned;
                }

                return (
                    userNameClaim,
                    emailClaim,
                    fullNameClaim,
                    phoneNumberClaim,
                    genderClaim,
                    idCardClaim,
                    birthday,
                    imageAccountClaim,
                    roleClaim,
                    isDelete,
                    timeBanned
                );
            }

            throw new UnauthorizedAccessException("Token không hợp lệ hoặc đã hết hạn. Vui lòng đăng nhập lại.");
        }
    }
}
