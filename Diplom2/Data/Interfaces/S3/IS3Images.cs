namespace Diplom2.Data.Interfaces.S3
{
    public interface IS3Images
    {
        Task<string> UploadImageAsync(string categoryId, int productId, IFormFile file);
        Task<MemoryStream> LoadImageAsync(string categoryId, int productId, string fileName);
        Task<IEnumerable<string>> List(string categoryId, int productId);
    }
}
