using Diplom2.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Diplom2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private const string imageUrl = "/api/Images/Category";
        private readonly IProduct _products;
        private readonly INiceness _niceness;
        private readonly IUser _user;

        public ProductsController(IProduct products, INiceness niceness, IUser user) { 
            _products = products;
            _niceness = niceness;
            _user = user;
        }

        [HttpGet("{id}")]
        public async Task<object> GetProduct(int id) {
            var product = await _products.Get(id);
            var user = await _user.Get(User, HttpContext.Connection.RemoteIpAddress);
            product.Tags.ForEach(t => _niceness.Increase(user, t, 1));
            return new { 
                id = product.Id, 
                name = product.Name,
                description = product.Description, 
                price = product.Price, 
                //images = Enumerable.Range(0,product.ImagesCount)
                //    .Select(i => $"{imageUrl}/{product.Category.Name}/{product.Id}/{i}.jpg"), 
                images = $"{imageUrl}/{product.Category.Id}/product/{product.Id}/image/0.jpg",
                specs = product.Specs, 
                categoryId = product.Category.Id 
            };
        }
    }
}
