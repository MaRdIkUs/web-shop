using Diplom2.Data.Models;
using System.Net;

namespace Diplom2.Data.Interfaces
{
    public interface IRecomender
    {
        public Task<IEnumerable<uint>> RecomendMain(User user, uint page, uint perPage, List<Models.Filter> filters, string? search);
        public Task<bool> BatchInsert(IEnumerable<Product> products);
        public Task<bool> Update(int id, Product products);
        public Task<bool> Delete(int id);
    }
}
