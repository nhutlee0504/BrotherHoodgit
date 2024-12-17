using API.Dto;
using API.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IRating
    {
        public Task<Rating> AddRating(int billDetailId, int star, string comment, IFormFile image);
        Task<IEnumerable<RatingDto>> GetRatings(int productId); // Sử dụng DTO cho kết quả
        public Task<IEnumerable<Rating>> GetRatingUser(string username);
    }
}
