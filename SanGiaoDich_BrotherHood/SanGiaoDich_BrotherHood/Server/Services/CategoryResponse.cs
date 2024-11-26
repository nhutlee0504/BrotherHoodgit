
using Microsoft.EntityFrameworkCore;
using SanGiaoDich_BrotherHood.Server.Data;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class CategoryResponse : ICategory
    {
        private readonly ApplicationDbContext _context;
        public CategoryResponse(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Category> AddCategory(Category category)
        {
            try
            {
                category.CreatedDate = DateTime.Now;  
                category.UpdatedDate = DateTime.Now; 
                category.UserUpdated = "Admin"; 

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                return category;
            }
            catch (Exception)
            {
                // Nếu có lỗi, trả về null
                return null;
            }
        }


        public async Task<Category> DeleteCategory(int IDCate)
        {
            var cate = await _context.Categories.FindAsync( IDCate);
            if (cate == null) 
                return null;
            _context.Categories.Remove(cate);
            await _context.SaveChangesAsync();
            return cate;
        }

        public async Task<Category> GeCategory(int IDCate)
        {
           return IDCate == null? null : await _context.Categories.FindAsync( IDCate);
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> UpdateCategory(int IDCate, Category category)
        {
            try
            {
                var cate = await _context.Categories.FindAsync(IDCate);
                if (cate == null)
                    return null;
                cate.NameCate = category.NameCate;
                _context.Categories.Update(cate);
                await _context.SaveChangesAsync();
                return cate;
            }
            catch (System.Exception)
            {

                return null;
            }
        }
        public async Task<IEnumerable<Category>> GetCategoryByName(string name)
        {
            var cateFind = await _context.Categories.Where(c => c.NameCate.Contains(name)).ToListAsync();
            if(cateFind == null)
            {
                throw new NotImplementedException("Không tìm thấy sản phẩm đang tìm kiếm");
            }
            return cateFind;
        }
    }
}
