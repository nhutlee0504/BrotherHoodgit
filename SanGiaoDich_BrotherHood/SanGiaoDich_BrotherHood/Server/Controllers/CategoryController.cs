
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanGiaoDich_BrotherHood.Server.Services;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections;
using System.Collections.Generic;
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
        public async Task<ActionResult> GetCategoryByName(string name)
        {
            // Gọi dịch vụ để tìm danh mục theo tên
            var categoryResult = await category.GetCategoryByName(name);

            // Kiểm tra nếu không tìm thấy
            if (categoryResult == null)
            {
                return NotFound($"Không tìm thấy danh mục với tên: {name}");
            }

            // Bao bọc kết quả trong danh sách và trả về
            return Ok(new List<Category> { categoryResult });
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


        [HttpPut("IDCate")]
        public async Task<ActionResult> UpdateCategory(int IDCate, Category cate)
        {
            return Ok(await category.UpdateCategory(IDCate,cate));
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
