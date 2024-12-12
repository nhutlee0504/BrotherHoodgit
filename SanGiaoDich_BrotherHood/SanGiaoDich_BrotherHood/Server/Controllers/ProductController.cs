﻿
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanGiaoDich_BrotherHood.Server.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using SanGiaoDich_BrotherHood.Server.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProduct prod;

        public ProductController(IProduct prods)
        {
            prod = prods;
        }
        [AllowAnonymous]
        [HttpGet("GetAllProduct")]
        public async Task<IActionResult> GetAllProduct()
        {
            try
            {
                var products = await prod.GetAllProductsAsync();
                return Ok(products);
            }
            catch (NotImplementedException ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetProductById/{id}")]
        public async Task<Product> GetProductById(int id)
        {
            return await prod.GetProductById(id);
        }

        [HttpGet("GetProductByName/{name}")]
        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            return await prod.GetProductByName(name);
        }

        [HttpPost]
        [Route("AddProduct")]
        public async Task<IActionResult> AddProduct(ProductDto productDto)
        {
            try
            {
                var product = await prod.AddProduct(productDto);
                return Ok(product);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Log the exception as needed
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("UpdateProduct/{Id}")]// Specify that {id} is a route parameter for the update method
        public async Task<IActionResult> UpdateProductById(int id, ProductDto productDto)//Cập nhật product
        {

            try
            {
                var updatedProduct = await prod.UpdateProductById(id, productDto);
                return Ok(updatedProduct);
            }
            catch (NotImplementedException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Log the exception as needed
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, "An error occurred while updating the product.");
            }
        }
        [HttpPut("Accept/{id}")]
        public async Task<IActionResult> Accept(int id)
        {

            try
            {
                var updatedProduct = await prod.AcceptProduct(id);
                return Ok(updatedProduct);
            }
            catch (NotImplementedException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the product.");
            }
        }
        [HttpPut("Cancle/{id}")]
        public async Task<IActionResult> Cancle(int id)
        {

            try
            {
                var updatedProduct = await prod.CancleProduct(id);
                return Ok(updatedProduct);
            }
            catch (NotImplementedException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the product.");
            }
        }

        [HttpGet]
        [Route("GetProductByNameAccount/{username}")]
        public async Task<IActionResult> GetProductByNameAccount(string username)
        {
            try
            {
                var products = await prod.GetProductByNameAccount(username);
                return Ok(products);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, "An error occurred while retrieving the products.");
            }
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var deletedProduct = await prod.DeleteProductById(id);
                return Ok(deletedProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã xảy ra lỗi khi xóa sản phẩm.");
            }
        }

        [HttpPut("UpgradeProrityLevel/{id}")]
        public async Task<IActionResult> UpgradeProrityLevel(int id)
        {
            try
            {
                var upgradedProduct = await prod.UpdateProrityLevel(id);
                return Ok(upgradedProduct);
            }
            catch (NotImplementedException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, "An error occurred while upgrading the product priority level.");
            }
        }
        [HttpGet("StatisticsByStatus")]
        public async Task<IActionResult> GetStatisticsByStatus()
        {
            var statistics = await prod.GetStatisticsByStatusAsync();
            return Ok(statistics);
        }

        [HttpGet("GetTotalRevenue")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            try
            {
                // Lấy tổng doanh thu từ service
                var totalRevenue = await prod.GetTotalRevenueAsync();
                return Ok(totalRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tính toán doanh thu: {ex.Message}");
            }
        }

        [HttpGet("GetRevenueByDate/{date}")]
        public async Task<IActionResult> GetRevenueByDate(DateTime date)
        {
            try
            {
                var revenue = await prod.GetRevenueByDateAsync(date);
                return Ok(revenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tính doanh thu theo ngày: {ex.Message}");
            }
        }

        [HttpGet("GetRevenueByWeek/{startDate}")]
        public async Task<IActionResult> GetRevenueByWeek(DateTime startDate)
        {
            try
            {
                var revenue = await prod.GetRevenueByWeekAsync(startDate);
                return Ok(revenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tính doanh thu theo tuần: {ex.Message}");
            }
        }

        [HttpGet("GetRevenueByMonth/{month}/{year}")]
        public async Task<IActionResult> GetRevenueByMonth(int month, int year)
        {
            try
            {
                var revenue = await prod.GetRevenueByMonthAsync(month, year);
                return Ok(revenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tính doanh thu theo tháng: {ex.Message}");
            }
        }
        [HttpGet("export-revenue")]
        public async Task<IActionResult> ExportRevenue()
        {
            try
            {
                // Fetch the revenue data for the current day and month
                var dailyRevenue = await prod.GetRevenueByDateAsync(DateTime.Now);
                var monthlyRevenue = await prod.GetRevenueByMonthAsync(DateTime.Now.Month, DateTime.Now.Year);

                // Create a list to hold the revenue data
                var revenueData = new List<RevenueExportModel>
        {
            new RevenueExportModel
            {
                Period = "Doanh Thu Theo Ngày",
                Revenue = dailyRevenue // Ensure using decimal zero (0m)
            },
            new RevenueExportModel
            {
                Period = "Doanh Thu Theo Tháng",
                Revenue = monthlyRevenue // Ensure using decimal zero (0m)
            }
        };

                // Export the data to Excel
                var excelFile = await ExportRevenueToExcelAsync(revenueData);

                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RevenueStatistics.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xuất doanh thu: {ex.Message}");
            }
        }



        // Helper method to export revenue data to Excel
        private async Task<byte[]> ExportRevenueToExcelAsync(List<RevenueExportModel> revenueData)
        {
            // You can use a library like EPPlus to create Excel files. Below is a basic example:

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Revenue Statistics");

                // Add header row
                worksheet.Cells[1, 1].Value = "Thời Gian";
                worksheet.Cells[1, 2].Value = "Doanh Thu";

                // Add data rows
                for (int i = 0; i < revenueData.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = revenueData[i].Period;
                    worksheet.Cells[i + 2, 2].Value = revenueData[i].Revenue.ToString("C");
                }

                // Return the file content as byte array
                return await Task.FromResult(package.GetAsByteArray());
            }
        }

        // Revenue Export Model
        public class RevenueExportModel
        {
            public string Period { get; set; }
            public decimal Revenue { get; set; }
        }

    }
}
