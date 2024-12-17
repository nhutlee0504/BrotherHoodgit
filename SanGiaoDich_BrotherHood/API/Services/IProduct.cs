using API.Dto;
using API.Models;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IProduct
    {
        public Task<IEnumerable<Product>> GetAllProductsAsync();
        public Task<IEnumerable<Product>> GetProductsAccount();
        public Task<IEnumerable<Product>> GetProductByNameAccount(string username);
        public Task<Product> GetProductById(int id);
        public Task<IEnumerable<Product>> GetProductByName(string name);
        public Task<Product> AddProduct(ProductDto product);
        public Task<Product> UpdateProductById(int id, ProductDto product);
        public Task<Product> AcceptProduct(int idproduct);
        public Task<Product> CancleProduct(int idproduct);
        public Task<Product> DeleteProductById(int id);
        Task<IEnumerable<dynamic>> GetStatisticsByStatusAsync();
    }
}
