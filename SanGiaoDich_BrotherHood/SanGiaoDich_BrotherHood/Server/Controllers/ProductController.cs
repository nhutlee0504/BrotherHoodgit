
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
using Microsoft.EntityFrameworkCore;
using SanGiaoDich_BrotherHood.Server.Data;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProduct prod;
        private readonly ApplicationDbContext _context;

        public ProductController(IProduct prods, ApplicationDbContext context)
        {
            prod = prods;
            _context = context;
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
        // API lọc theo ngày
        [HttpGet("GetProductsByDate")]
        public IActionResult GetProductsByDate(DateTime startDate, DateTime endDate)
        {
            // Điều chỉnh endDate để bao gồm cả dữ liệu ngày kết thúc
            endDate = endDate.Date.AddDays(1).AddTicks(-1);

            var products = _context.Products
                .Where(p => p.StartDate >= startDate && p.StartDate <= endDate)
                .ToList();

            return Ok(products);
        }

    }
}
