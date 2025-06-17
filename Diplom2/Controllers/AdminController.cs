using Diplom2.Data.Interfaces;
using Diplom2.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diplom2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IProduct _products;
        private readonly ICategory _category;
        private readonly IFilter _filter;
        private readonly IUser _user;
        private readonly ICart _cart;

        public AdminController(IProduct product, ICategory category, IFilter filter, IUser user, ICart cart) { 
            _products = product;
            _category = category;
            _filter = filter;
            _user = user;
            _cart = cart;
        }
        
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _products.GetAll();
            var result = products.Select(p => new
            {
                id = p.Id,
                name = p.Name,
                description = p.Description,
                price = p.Price,
                count = p.Count,
                categoryId = p.Category?.Id,
                specs = (dynamic)(p.Specs != null ? p.Specs.Select(s => new { name = s.Name, value = s.Value }).ToList() : new List<object>()),
                tags = (dynamic)(p.Tags != null ? p.Tags.Select(t => new { name = t.Name, value = t.Value }).ToList() : new List<object>())
            });
            return Ok(result);
        }

        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto dto)
        {
            var category = await _category.Get(dto.categoryId);
            if (category == null)
                return BadRequest(new { error = "Category not found" });

            var product = new Product
            {
                Name = dto.name,
                Description = dto.description,
                Price = dto.price,
                Count = dto.count,
                Category = category,
                Specs = dto.specs?.Select(s => new Specs { Name = s.name, Value = s.value }).ToList() ?? new List<Specs>(),
                Tags = dto.tags?.Select(t => new Tag { Name = t.name, Value = t.value }).ToList() ?? new List<Tag>(),
                ImagesCount = 0,
                Popularirty = 0
            };

            await _products.Insert(product);
                return Ok(new
                {
                    id = 0,
                    name = product.Name,
                    description = product.Description,
                    price = product.Price,
                    count = product.Count,
                    categoryId = product.Category.Id,
                    specs = (dynamic)(product.Specs != null ? product.Specs.Select(s => new { name = s.Name, value = s.Value }).ToList() : new List<object>()),
                    tags = (dynamic)(product.Tags != null ? product.Tags.Select(t => new { name = t.Name, value = t.Value }).ToList() : new List<object>())
                });
        }

        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto dto)
        {
            var existingProduct = await _products.Get(id);
            if (existingProduct == null)
                return NotFound(new { error = "Product not found" });

            var category = await _category.Get(dto.categoryId);
            if (category == null)
                return BadRequest(new { error = "Category not found" });

            var product = new Product
            {
                Id = id,
                Name = dto.name,
                Description = dto.description,
                Price = dto.price,
                Count = dto.count,
                Category = category,
                Specs = dto.specs?.Select(s => new Specs { Name = s.name, Value = s.value }).ToList() ?? new List<Specs>(),
                Tags = dto.tags?.Select(t => new Tag { Name = t.name, Value = t.value }).ToList() ?? new List<Tag>(),
                ImagesCount = existingProduct.ImagesCount,
                Popularirty = existingProduct.Popularirty
            };

            var result = await _products.Update(id, product);
            if (result > 0)
            {
                return Ok(new
                {
                    id = product.Id,
                    name = product.Name,
                    description = product.Description,
                    price = product.Price,
                    count = product.Count,
                    categoryId = product.Category.Id,
                    specs = (dynamic)(product.Specs != null ? product.Specs.Select(s => new { name = s.Name, value = s.Value }).ToList() : new List<object>()),
                    tags = (dynamic)(product.Tags != null ? product.Tags.Select(t => new { name = t.Name, value = t.Value }).ToList() : new List<object>())
                });
            }
            return BadRequest(new { error = "Failed to update product" });
        }

        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _products.Delete(id);
            if (result > 0)
                return NoContent();
            return NotFound(new { error = "Product not found" });
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _category.GetAll();
            var result = categories.Select(c => new
            {
                id = c.Id,
                name = c.Name,
                description = c.Name
            });
            return Ok(result);
        }

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto dto)
        {
            var category = new Category
            {
                Name = dto.name
            };

            var result = await _category.Insert(category);
            if (result > 0)
            {
                return Ok(new
                {
                    id = result,
                    name = category.Name,
                    description = category.Name
                });
            }
            return BadRequest(new { error = "Failed to create category" });
        }

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto dto)
        {
            var existingCategory = await _category.Get(id);
            if (existingCategory == null)
                return NotFound(new { error = "Category not found" });

            var category = new Category
            {
                Id = id,
                Name = dto.name,
                Filters = existingCategory.Filters ?? new List<Filter>()
            };

            var result = await _category.Update(id, category);
            if (result > 0)
            {
                return Ok(new
                {
                    id = category.Id,
                    name = category.Name,
                    description = category.Name
                });
            }
            return BadRequest(new { error = "Failed to update category" });
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _category.Delete(id);
            if (result > 0)
                return NoContent();
            return NotFound(new { error = "Category not found" });
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var users = await _user.GetAll();
            var orders = new List<object>();
            
            foreach (var user in users)
            {
                foreach (var history in user.History)
                {
                    orders.Add(new
                    {
                        id = history.Id,
                        userId = user.UUID,
                        status = "completed",
                        total = history.Purchase.Price * history.Quantity,
                        createdAt = history.Date
                    });
                }
            }
            
            return Ok(orders);
        }

        [HttpPut("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatusUpdateDto dto)
        {
            return Ok(new
            {
                id = id,
                status = dto.status,
                updatedAt = DateTime.UtcNow
            });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _user.GetAll();
            var result = users.Select(u => new
            {
                id = u.UUID,
                email = u.UUID,
                name = u.UUID,
                isActive = true,
                createdAt = DateTime.UtcNow
            });
            return Ok(result);
        }

        [HttpPut("users/{id}/status")]
        public async Task<IActionResult> UpdateUserStatus(string id, [FromBody] UserStatusUpdateDto dto)
        {
            var user = await _user.Get(id);
            if (user == null)
                return NotFound(new { error = "User not found" });

            return Ok(new
            {
                id = id,
                isActive = dto.isActive,
                updatedAt = DateTime.UtcNow
            });
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var users = await _user.GetAll();
            var products = await _products.GetList(new List<uint>());
            
            var totalProducts = products.Count();
            var totalUsers = users.Count;
            
            var totalOrders = users.Sum(u => u.History.Count);
            var totalRevenue = users.Sum(u => u.History.Sum(h => h.Purchase.Price * h.Quantity));

            return Ok(new
            {
                totalProducts,
                totalOrders,
                totalUsers,
                totalRevenue
            });
        }
    }

    public class CategoryDto
    {
        public string name { get; set; }
        public string description { get; set; }
        public FilterDto[]? filters { get; set; }
    }

    public class FilterDto 
    {
        public FilterType Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string DefaultValue { get; set; }
        public Filter ToFilter() {
            return new() { Type = Type, Name = Name, Value = Value, DefaultValue = DefaultValue };
        }
    }

    public class ProductCreateDto
    {
        public string name { get; set; }
        public string description { get; set; }
        public double price { get; set; }
        public uint count { get; set; }
        public int categoryId { get; set; }
        public SpecDto[] specs { get; set; }
        public TagDto[] tags { get; set; }
    }

    public class ProductUpdateDto
    {
        public string name { get; set; }
        public string description { get; set; }
        public double price { get; set; }
        public uint count { get; set; }
        public int categoryId { get; set; }
        public SpecDto[] specs { get; set; }
        public TagDto[] tags { get; set; }
    }

    public class CategoryCreateDto
    {
        public string name { get; set; }
        public string description { get; set; }
    }

    public class CategoryUpdateDto
    {
        public string name { get; set; }
        public string description { get; set; }
    }

    public class OrderStatusUpdateDto
    {
        public string status { get; set; }
        public string? comment { get; set; }
    }

    public class UserStatusUpdateDto
    {
        public bool isActive { get; set; }
    }

    public class SpecDto
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class TagDto
    {
        public string name { get; set; }
        public string value { get; set; }
    }
}
