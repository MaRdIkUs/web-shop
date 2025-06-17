using Diplom2.Data.Models;

namespace Diplom2.Data.Interfaces
{
    public interface IFilter
    {
        public Filter? Get(int id);
        public List<Filter> Get(string name);
        public void Add(Filter filter);
        public void Update(int id, Filter filter);
        public bool Delete(int id);
    }
}
