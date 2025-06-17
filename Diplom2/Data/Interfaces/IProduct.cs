using Diplom2.Data.Models;

namespace Diplom2.Data.Interfaces
{
    public interface IProduct
    {
        public Task<IEnumerable<Product>> GetList(IEnumerable<uint> productsIds);
        public Task Insert(Product product);
        public Task<int> Update(int id, Product product);
        public Task<int> Delete(int id);
        public Task<Product> Get(int id);
        public Task<IEnumerable<Product>> GetAll();
    }
}
