using Diplom2.Data.Interfaces;
using Diplom2.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Diplom2.Data.DBO
{
    public class CategoryRepository : ICategory
    {
        private ApplicationDbContext _context;
        private readonly IFilter _filter;
        public CategoryRepository(ApplicationDbContext context, IFilter filter) {
            _context = context;
            _filter = filter;
        }
        public async Task<int> Delete(int id)
        {
            _context.Categories.Remove(_context.Categories.First(c => c.Id == id));
            return await _context.SaveChangesAsync();
        }

        public async Task<Category> Get(int id)
        {
            return _context.Categories.First(c => c.Id == id);
        }

        public async Task<List<Category>> GetAll()
        {
            return _context.Categories.ToList();
        }

        public async Task<int> Insert(Category category)
        {
            _context.Categories.Add(category);
            _filter.Add(new() { Name = category.Name, Type = FilterType.Text, Value = category.Name, DefaultValue = "1" });
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Update(int id, Category category)
        {
            _context.Categories.First(c => c.Id == id).Copy(category);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<Filter>> GetFilters(int id)
        {
            return _context.Categories.Include(c => c.Filters).First(c => c.Id == id).Filters;
        }
    }
}
