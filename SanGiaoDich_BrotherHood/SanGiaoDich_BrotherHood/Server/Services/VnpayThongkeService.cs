using LiteDB;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SanGiaoDich_BrotherHood.Server.Data;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class VnpayThongkeService : IVnpayThongkeService
    {
        private readonly ApplicationDbContext _context;

        public VnpayThongkeService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<PaymentRequestModel>> GetAllPaymentRequestsAsync()
        {
            return await _context.PaymentRequests.OrderByDescending(c => c.CreatedDate)
               .ToListAsync();
        }


        public async Task<List<PaymentRequestModel>> GetAllPaymentRequestsWithUser(string username)
        {
            var paymentRequests = await _context.PaymentRequests
                .Include(pr => pr.PaymentResponse) 
                .Where(pr => pr.UserName == username)
                .OrderByDescending(a =>a.CreatedDate)
                .Select(pr => new PaymentRequestModel
                {
                    PaymentReqID = pr.PaymentReqID,
                    UserName = pr.UserName,
                    TxnRef = pr.TxnRef,
                    Amount = pr.Amount,
                    OrderDescription = pr.OrderDescription,
                    CreatedDate = pr.CreatedDate,
                    PaymentType = pr.PaymentType,

                    PaymentResponse = pr.PaymentResponse != null && pr.PaymentResponse.PaymentReqID == pr.PaymentReqID
                        ? new PaymentResponseModel
                        {
                            Success = pr.PaymentResponse.Success,
                            PaymentMethod = pr.PaymentResponse.PaymentMethod,
                            VnPayResponseCode = pr.PaymentResponse.VnPayResponseCode,
                            PaymentReqID = pr.PaymentResponse.PaymentReqID
                        }
                        : null
                })
                .ToListAsync();

            return paymentRequests;
        }

        public async Task<List<PaymentResponseModel>> GetAllPaymentResponsesAsync()
        {
            return await _context.PaymentResponses
               .ToListAsync();
        }
    }
}
