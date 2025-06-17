using Diplom2.Data.Interfaces;
using Diplom2.Data.Models;

namespace Diplom2.Data.DBO
{
    public class FilterRepository : IFilter
    {
        private readonly ApplicationDbContext _context;
        public FilterRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Add(Filter filter)
        {
            _context.Filters.Add(filter);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            _context.Filters.Remove(_context.Filters.Where(filter => filter.Id == id).First());
            _context.SaveChanges();
            return true;
        }

        public Filter? Get(int id)
        {
            return _context.Filters.Where(f => f.Id == id).First();
        }

        public List<Filter> Get(string name)
        {
            return _context.Filters.Where(f => f.Name == name).ToList();
        }

        public void Update(int id, Filter filter)
        {
            Delete(id);
            Add(filter);
        }
    }
}
