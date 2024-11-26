using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
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
        public async Task<ActionResult> GetCategories()
        {
            return Ok(await category.GetCategories());
        }

        [HttpGet("IDCate")]
        public async Task<ActionResult> GetCategoryByID(int IDCate)
        {
            var result = await category.GeCategory(IDCate);
            if (result == null)
                return NotFound($"Không tìm thấy danh mục với ID: {IDCate}");
            return Ok(result);
        }

        [HttpGet("Name")]
        public async Task<ActionResult> GetCategoryByName(string name)
        {
            var categoryResult = await category.GetCategoryByName(name);
            if (categoryResult == null)
                return NotFound();
            return Ok(categoryResult);
        }


        [HttpPost]
        public async Task<ActionResult> AddCategory(string cate)
        {
            var ct = await category.AddCategory(cate);
            if (ct == null)
                return BadRequest();
            return CreatedAtAction("AddCategory",ct);
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
