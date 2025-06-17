using Diplom2.Data.Models;

namespace Diplom2.Data.Interfaces
{
    public interface ICategory
    {
        public Task<Category> Get(int id);
        public Task<List<Category>> GetAll();
        public Task<int> Insert(Category category);
        public Task<int> Update(int id, Category category);
        public Task<int> Delete(int id);
        public Task<List<Filter>> GetFilters(int id);
    }
}
