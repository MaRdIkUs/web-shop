using Diplom2.Data.Interfaces;
using Diplom2.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Diplom2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICart _cart;

        public CartController(ICart cart, IUser users) {
            _cart = cart;
        }

        private string GetUUID()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpGet(Name = "GetCart")]
        public async Task<IEnumerable<object>> Get()
        {
            var cart = await _cart.Get(GetUUID());
            return cart.Select(x => new { 
                itemId = x.Id, 
                productId = x.ProductId, 
                quantity = x.Quantity 
            });
        }

        [HttpPost(Name = "PostCart")]
        public async Task<IActionResult> Post([FromBody] Purchase purchase) {
            var cart = await _cart.Get(GetUUID());
            cart.Add(purchase);
            return Ok(purchase);
        }

        [HttpPut("{itemId}")]
        public async Task<IActionResult> Put(int itemId, [FromBody] Quantity quantity) {
            var result = await _cart.EditQuantity(GetUUID(), itemId, quantity.quantity);
            return Ok(result);
        }

        [HttpDelete("{itemId}")]
        public async Task<IActionResult> Delete(int itemId) {
            var result = await _cart.Delete(GetUUID(), itemId);
            return Ok(result);
        }
    }
    public class Quantity
    {
        public int quantity { get; set; }
    }
}
