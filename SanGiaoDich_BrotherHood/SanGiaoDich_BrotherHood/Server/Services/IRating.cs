
using Microsoft.AspNetCore.Http;
using SanGiaoDich_BrotherHood.Server.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface IRating
    {
        public Task<Rating> AddRating(int billDetailId, int star, string comment, IFormFile image);
        Task<IEnumerable<RatingDto>> GetRatings(int productId); // Sử dụng DTO cho kết quả
    }
}
