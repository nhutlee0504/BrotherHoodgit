
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanGiaoDich_BrotherHood.Server.Services;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategory category;
        public CategoryController(ICategory category)
        {
            this.category = category;
        }

        [HttpGet]
        [Route("GetCategories")]
        public async Task<ActionResult> GetCategories()
        {
            return Ok(await category.GetCategories());
        }

        [HttpGet]
        [Route("GetCategoryByID/{IDCate}")]
        public async Task<ActionResult> GetCategoryByID(int IDCate)
        {
            var result = await category.GeCategory(IDCate);
            if (result == null)
                return NotFound($"Không tìm thấy danh mục với ID: {IDCate}");
            return Ok(result);
        }

        [HttpGet]
        [Route("GetCategoryByName/{name}")]
        public async Task<IActionResult> GetCategoryByName(string name)
        {
            try
            {
                // Gọi dịch vụ để tìm danh mục theo tên
                var categoryResult = await category.GetCategoryByName(name);

                // Trả về danh sách kết quả
                return Ok(categoryResult);
            }
            catch (NotImplementedException ex)
            {
                // Xử lý ngoại lệ khi không tìm thấy danh mục
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ chung
                return StatusCode(500, $"Đã xảy ra lỗi: {ex.Message}");
            }
        }


        [HttpPost("AddCategory")]
        public async Task<ActionResult> AddCategory(Category cate)
        {
        
            var ct = await category.AddCategory(new Category
            {
                NameCate = cate.NameCate
            });
            if (ct == null)
            {
                return BadRequest("Không thể tạo loại mới.");
            }
            return CreatedAtAction(nameof(AddCategory), new { id = ct.IDCategory }, ct);
        }


        [HttpPut("UpdateCategory/{IDCate}")]
        public async Task<ActionResult> UpdateCategory(int IDCate, Category cate)
        {
            var existingCategory = await category.GeCategory(IDCate);
            if (existingCategory == null)
            {
                return NotFound($"Không tìm thấy danh mục với ID: {IDCate}");
            }

            existingCategory.NameCate = cate.NameCate;
            existingCategory.UpdatedDate = DateTime.Now;
            existingCategory.UserUpdated = "Admin"; // Thay đổi theo user thực tế

            var updatedCategory = await category.UpdateCategory(IDCate, existingCategory);
            return Ok(updatedCategory);
        }


        [HttpDelete("IDCate")]
        public async Task<ActionResult> DeleteCategory(int IDCate)
        {
            var ct = await category.DeleteCategory(IDCate);
            if (ct == null)
                return BadRequest();
            return NoContent();
        }
    }
}
