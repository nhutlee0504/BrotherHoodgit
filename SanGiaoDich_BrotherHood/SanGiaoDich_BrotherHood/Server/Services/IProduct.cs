
using Microsoft.AspNetCore.Http;
using SanGiaoDich_BrotherHood.Client.Pages;
using SanGiaoDich_BrotherHood.Server.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        public Task<Product> UpdateProductById(int id, ProductDto product);
        public Task<Product> AcceptProduct(int idproduct);
        public Task<Product> CancleProduct(int idproduct);
        public Task<Product> DeleteProductById(int id);
        public Task<Product> UpdateProrityLevel(int id);
        public Task<IEnumerable<dynamic>> GetStatisticsByStatusAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetRevenueByDateAsync(DateTime date);
        Task<decimal> GetRevenueByMonthAsync(int month, int year);
        Task<decimal> GetRevenueByYearAsync(int year);
        Task<MemoryStream> ExportRevenueStatisticsToExcelAsync(DateTime date);
        Task<int> GetApprovedPostsByMonthAsync(int month, int year);

        // Phương thức xuất báo cáo bài đăng sang Excel
        Task<MemoryStream> ExportAllStatisticsToExcelAsync();

    }
}
