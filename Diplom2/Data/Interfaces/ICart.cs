using Diplom2.Data.Models;

namespace Diplom2.Data.Interfaces
{
    public interface ICart
    {
        public Task<List<Purchase>> Get(string User_UUID);
        public Task<int> Add(string User_UUID, Purchase Purchase);
        public Task<int> Delete(string User_UUID, int ItemId);
        public Task<int> EditQuantity(string User_UUID, int ItemId, int quantity);
        public Task<int> DeleteAll(string User_UUID);
    }
}
