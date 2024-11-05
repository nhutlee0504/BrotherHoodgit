using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public class CategoryResponse : ICategory
    {
        private readonly ApplicationDbContext _context;
        public CategoryResponse(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Category> AddCategory(string nameCategory)
        {
            try
            {
                var newCate = new Category
                {
                    NameCate = nameCategory
                };
                await _context.Categories.AddAsync(newCate);
                await _context.SaveChangesAsync();
                return newCate;
            }
            catch (System.Exception)
            {

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
           return await _context.Categories.FindAsync( IDCate);
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
    }
}
