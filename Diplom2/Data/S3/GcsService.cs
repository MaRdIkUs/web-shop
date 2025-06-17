using Diplom2.Data.Interfaces.S3;
using Diplom2.Data.Models;
using Google.Cloud.Storage.V1;

namespace Diplom2.Data.S3
{
    public class GcsService : IS3Images
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private readonly string _imagesFolder;
        
        public GcsService(StorageClient storageClient, IConfiguration config)
        {
            _storageClient = storageClient;
            _bucketName = config.GetValue<string>("Gcs:BucketName") 
                ?? throw new ArgumentException("Gcs:BucketName is missing in configuration");
            _imagesFolder = config.GetValue<string>("Gcs:ImagesFolder")
                ?? throw new ArgumentException("Gcs:ImagesFolder is missing in configuration");
            _imagesFolder = _imagesFolder.TrimEnd('/') + '/';
        }

        public async Task<MemoryStream> LoadImageAsync(string categoryId, int productId, string fileName)
        {
            var memoryStream = new MemoryStream();
            await _storageClient.DownloadObjectAsync(
                bucket: _bucketName,
                objectName: convertPath(categoryId, productId, fileName),
                destination: memoryStream
            );
            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<string> UploadImageAsync(string categoryId, int productId, IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var objectName = convertPath(categoryId, productId, "0.jpg");
            await _storageClient.UploadObjectAsync(
                bucket: _bucketName,
                objectName: objectName,
                contentType: file.ContentType,
                source: stream
            ); 
            return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
        }

        public async Task<IEnumerable<string>> List(string categoryId, int productId)
        {
            var results = new List<string>();
            await foreach (var obj in _storageClient.ListObjectsAsync(_bucketName, $"{_imagesFolder}{categoryId}/{productId}"))
            {
                results.Add(obj.Name);
            }
            return results;
        }

        private string convertPath(string categoryId, int productId, string fileName) => $"{_imagesFolder}{categoryId}/{productId}/{fileName}";
    }
}
