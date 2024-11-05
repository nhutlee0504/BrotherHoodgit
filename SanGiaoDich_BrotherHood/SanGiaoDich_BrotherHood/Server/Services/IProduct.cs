
using Microsoft.AspNetCore.Http;
using SanGiaoDich_BrotherHood.Server.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface IProduct
    {
        public Task<IEnumerable<Product>> GetAllProductsAsync();
        public Task<IEnumerable<Product>> GetProductsAccount();
        public Task<IEnumerable<Product>> GetProductByNameAccount(string username);
        public Task<Product> GetProductById(int id);
        public Task<IEnumerable<Product>> GetProductByName(string name);
        public Task<Product> AddProduct(ProductDto product);
        Task<Product> UpdateProductById(int id, ProductDto product);
        public Task <Product> DeleteProductById(int id);
    }
}
