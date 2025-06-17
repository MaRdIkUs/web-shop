using Diplom2.Data.Interfaces;
using Diplom2.Data.Interfaces.S3;
using Microsoft.AspNetCore.Mvc;

namespace Diplom2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly IS3Images _images;
        private readonly IProduct _product;

        public ImagesController(IS3Images images, IProduct product) { 
            _images = images;
            _product = product;
        }

        [HttpGet("category/{categoryId}/product/{productId}/list")]
        public async Task<IActionResult> CategoryImageNames(string categoryId, int productId) {
            var images = _images.List(categoryId, productId);
            return Ok(await images);
        }

        [HttpGet("category/{categoryId}/product/{productId}/image/{fileName}")]
        public async Task<IActionResult> DownloadImage(string categoryId, int productId, string fileName)
        {
            try
            {
                var stream = await _images.LoadImageAsync(categoryId, productId, fileName);
                return File(stream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost("upload/category/{categoryId}/product/{productId}")]
        public async Task<IActionResult> UploadImage(string categoryId, int productId,[FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не передан или пустой");
            var product = await _product.Get(productId);
            product.ImagesCount += 1;
            await _product.Update(productId, product);
            return Ok(await _images.UploadImageAsync(categoryId, productId, file));
        }
    }
}
