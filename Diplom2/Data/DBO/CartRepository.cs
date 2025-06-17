using Diplom2.Data.Interfaces;
using Diplom2.Data.Models;
using System;

namespace Diplom2.Data.DBO
{
    public class CartRepository : ICart
    {
        private readonly ApplicationDbContext _context;
        private readonly IUser _user;
        public CartRepository(ApplicationDbContext context, IUser user)
        {
            _context = context;
            _user = user;
        }
        public async Task<int> Add(string User_UUID, Purchase Purchase)
        {
            _context.Users.First(u => u.UUID == User_UUID).Cart.Add(Purchase);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Delete(string User_UUID, int ItemId)
        {
            var cart = _context.Users.First(u => u.UUID == User_UUID).Cart;
            cart.Remove(cart.First(p => p.Id == ItemId));
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAll(string User_UUID)
        {
            _context.Users.First(u => u.UUID == User_UUID).Cart.Clear();
            return await _context.SaveChangesAsync();
        }

        public async Task<int> EditQuantity(string User_UUID, int ItemId, int quantity)
        {
            var cart = _context.Users.First(u => u.UUID == User_UUID).Cart;
            cart.First(p => p.Id == ItemId).Quantity = quantity;
            return await _context.SaveChangesAsync();
        }

        public async Task<List<Purchase>> Get(string User_UUID)
        {
            var userlist = _context.Users;
            User user;
            if (userlist.Count() == 0)
                user = await _user.Create(new User(User_UUID));
            else
                user = userlist.First(u => u.UUID == User_UUID);
            return user.Cart;
        }
    }
}
