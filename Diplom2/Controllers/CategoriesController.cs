using Diplom2.Data.DBO;
using Diplom2.Data.Interfaces;
using Diplom2.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Math;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Diplom2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        private const string imageUrl = "/api/Images/Category";
        private const int _defaultPageSize = 50;
        private readonly ICategory _categories;
        private readonly IRecomender _recomender;
        private readonly IUser _user;
        private readonly IFilter _filter;
        private readonly IProduct _product;

        public CategoriesController(ICategory categories, IRecomender recomender, ILocalGeoService localGeoService, IUser user, IFilter filter, IProduct product) { 
            _categories = categories;
            _recomender = recomender;
            _user = user;
            _filter = filter;
            _product = product;
        }

        [HttpGet(Name = "GetCategories")]
        public async Task<IEnumerable<object>> Get()
        {
            
            return (await _categories.GetAll()).Select(
                c => new { 
                    id = c.Id, 
                    name = c.Name 
                });
        }
        [HttpGet("{id}/filters")]
        public async Task<List<Filter>> GetFilter(int id) {
            return await _categories.GetFilters(id);
        }

        [HttpGet("{id}/products")]
        public async Task<IEnumerable<object>> GetCategoryProductsAsync(int id, [FromQuery] int page = 1, [FromQuery] int limit = _defaultPageSize, [FromQuery] string search = "", [FromQuery] string status = "")
        {
            var user = await _user.Get(User,HttpContext.Connection.RemoteIpAddress);
            var category = await _categories.Get(id);
            if (category == null)
                return Enumerable.Empty<object>();
            var filters = new List<Filter>() { _filter.Get(category.Name)[0] };
            var recomendations = await _recomender.RecomendMain(user, (uint)page, _defaultPageSize, filters, search);
            var products = await _product.GetList(recomendations);
            return products.Select(p => new { 
                id          = p.Id, 
                name        = p.Name, 
                price       = p.Price, 
                categoryId  = p.Category.Id,
                images    = $"{imageUrl}/{p.Category.Id}/product/{p.Id}/image/0.jpg"
            });
        }
    }
}
