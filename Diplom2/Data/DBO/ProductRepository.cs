using Diplom2.Data.Interfaces;
using Diplom2.Data.Models;
using Google.Apis.Json;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;
using System.Diagnostics;
using System.Linq;

namespace Diplom2.Data.DBO
{
    public class ProductRepository : IProduct
    {
        private readonly ApplicationDbContext _context;
        private readonly IRecomender _recomender;
        public ProductRepository(ApplicationDbContext context, IRecomender recomender) { 
            _context = context;
            _recomender = recomender;
        }
        public async Task<int> Delete(int id)
        {
            var product = await _context.Products
                .Include(p => p.Specs)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id);
                
            if (product != null)
            {
                _context.Products.Remove(product);
                await _recomender.Delete(id);
            }
            
            return await _context.SaveChangesAsync();
        }

        public async Task<Product> Get(int id)
        {
            return (await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Specs)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id))!;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Specs)
                .Include(p => p.Tags);
        }

        public async Task<IEnumerable<Product>> GetList(IEnumerable<uint> productsIds)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Specs)
                .Include(p => p.Tags)
                .Where(p => productsIds.Contains((uint)p.Id))
                .ToListAsync();
            var p = productsIds.ToList();
            return products.OrderBy(o => p.IndexOf((uint)o.Id)).ToList();
        }

        public async Task Insert(Product product)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            var p = _context.Products.Where(p => p.Name == product.Name).Include(p => p.Tags).First();
            await _recomender.BatchInsert(new List<Product>() { p });
        }

        public async Task<int> Update(int id, Product product)
        {
            var existingProduct = await _context.Products
                .Include(p => p.Specs)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id);
                
            if (existingProduct != null)
            {
                existingProduct.Copy(product);
                if (product.Specs != null)
                {
                    existingProduct.Specs.Clear();
                    existingProduct.Specs.AddRange(product.Specs);
                }
                if (product.Tags != null)
                {
                    existingProduct.Tags.Clear();
                    existingProduct.Tags.AddRange(product.Tags);
                }
            }
            try
            {
                await _recomender.Update(id, product);
            }
            catch (Exception _){}
            return await _context.SaveChangesAsync();
        }
    }
}
