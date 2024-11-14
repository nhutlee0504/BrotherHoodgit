
using Microsoft.AspNetCore.Http;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface IImageProduct
    {
        public Task<IEnumerable<ImageProduct>> GetImages();
        public Task<IEnumerable<ImageProduct>> GetImageProducts(int id);
        public Task DeleteImage(int idProd, int idImage);
        Task<ImageProduct> UpdateImage(int idImage, IFormFile file);
        Task<IEnumerable<ImageProduct>> UploadImages(List<IFormFile> files, int productId);
    }
}
