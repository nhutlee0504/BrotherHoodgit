using API.Dto;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private readonly IProduct prod;

		public ProductController(IProduct prods)
		{
			prod = prods;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllProduct()
		{
			try
			{
				var products = await prod.GetAllProductsAsync();
				return Ok(products);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Đã xảy ra lỗi khi lấy danh sách sản phẩm.");
			}
		}

		[HttpGet("GetProductId")]
		public async Task<IActionResult> GetProductById(int id)
		{
			try
			{
				var product = await prod.GetProductById(id);
				if (product == null || product.Status == "Đã xóa")
				{
					return NotFound("Sản phẩm không tồn tại hoặc đã bị xóa.");
				}
				return Ok(product);
			}
			catch (NotImplementedException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("name")]
		public async Task<IActionResult> GetProductByName(string name)
		{
			try
			{
				var products = await prod.GetProductByName(name);
				return Ok(products.Where(p => p.Status != "Đã xóa"));
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Đã xảy ra lỗi khi lấy sản phẩm theo tên.");
			}
		}

		[HttpPost("AddProduct")]
		public async Task<IActionResult> AddProduct([FromForm] ProductDto productDto)
		{
			try
			{
				var product = await prod.AddProduct(productDto);
				return Ok(product);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Đã xảy ra lỗi khi thêm sản phẩm.");
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
			catch (Exception ex)
			{
				return StatusCode(500, "Đã xảy ra lỗi khi cập nhật sản phẩm.");
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateProductById(int id, [FromForm] ProductDto productDto)
		{
			try
			{
				var updatedProduct = await prod.UpdateProductById(id, productDto);
				return Ok(updatedProduct);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Đã xảy ra lỗi khi cập nhật sản phẩm.");
			}
		}

		[HttpGet("GetProductByNameAccount/{username}")]
		public async Task<IActionResult> GetProductByNameAccount(string username)
		{
			try
			{
				var products = await prod.GetProductByNameAccount(username);
				return Ok(products.Where(p => p.Status != "Đã xóa"));
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Đã xảy ra lỗi khi lấy sản phẩm theo tài khoản.");
			}
		}

		[HttpPost("delete/{id}")]
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
		[HttpGet("statistics")]
		public async Task<IActionResult> GetStatisticsByStatus()
		{
			try
			{
				var products = await prod.GetAllProductsAsync();

				if (products == null || !products.Any())
				{
					return NotFound("Không có sản phẩm nào trong hệ thống.");
				}

				var statistics = products
					.GroupBy(p => p.Status)
					.Select(group => new
					{
						Status = group.Key,
						Count = group.Count()
					})
					.ToList();

				return Ok(statistics);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Đã xảy ra lỗi khi thống kê bài đăng.");
			}
		}

	}
}
