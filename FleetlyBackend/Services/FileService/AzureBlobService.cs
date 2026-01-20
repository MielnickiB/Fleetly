using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace FleetlyBackend.Services.FileService
{
    public class AzureBlobService : IFileService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly long _maxSize;
        private readonly string[] _allowedExtensions;

        private const string ContainerName = "uploads";

        private const int MaxImageWidth = 5000;
        private const int JpegQuality = 85;
        private const int MagicNumberBufferSize = 12;

        public AzureBlobService(IConfiguration configuration, IOptions<FileUploadOptions> options)
        {
            var connectionString = configuration.GetConnectionString("AzureStorage");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("Brak ConnectionString 'AzureStorage' w konfiguracji.");
            }

            _blobServiceClient = new BlobServiceClient(connectionString);

            _maxSize = options.Value.MaxFileSize;
            _allowedExtensions = options.Value.AllowedExtensions;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string pathPrefix)
        {
            ValidateSize(file);
            ValidateExtension(file);
            await ValidateMagicNumbers(file);

            var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var ext = Path.GetExtension(file.FileName).ToLower();

            var finalExtension = (ext == ".pdf") ? ".pdf" : (ext == ".png" ? ".png" : ".jpg");

            var uniqueFileName = $"{Guid.NewGuid()}{finalExtension}";

            var blobName = string.IsNullOrEmpty(pathPrefix)
                ? uniqueFileName
                : $"{pathPrefix}/{uniqueFileName}".Replace("\\", "/");

            var blobClient = containerClient.GetBlobClient(blobName);
            var blobHeaders = new BlobHttpHeaders { ContentType = file.ContentType };

            if (finalExtension == ".pdf")
            {
                await using var stream = file.OpenReadStream();
                if (stream.CanSeek) stream.Position = 0;
                await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHeaders });
            }
            else
            {
                using var memoryStream = new MemoryStream();

                await CompressImageToStream(file, memoryStream, finalExtension);

                memoryStream.Position = 0;

                if (finalExtension == ".jpg") blobHeaders.ContentType = "image/jpeg";
                if (finalExtension == ".png") blobHeaders.ContentType = "image/png";

                await blobClient.UploadAsync(memoryStream, new BlobUploadOptions { HttpHeaders = blobHeaders });
            }

            return blobClient.Uri.ToString();
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            try
            {
                var uri = new Uri(fileUrl);

                var path = uri.LocalPath;

                var prefixToRemove = $"/{ContainerName}/";

                if (path.StartsWith(prefixToRemove))
                {
                    var blobName = path.Substring(prefixToRemove.Length);

                    blobName = System.Net.WebUtility.UrlDecode(blobName);

                    var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
                    var blobClient = containerClient.GetBlobClient(blobName);

                    await blobClient.DeleteIfExistsAsync();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Nie udało się usunąć pliku z Azure Blob Storage.");
            }
        }

        private async Task CompressImageToStream(IFormFile file, Stream outputStream, string extension)
        {
            await using var inputStream = file.OpenReadStream();
            if (inputStream.CanSeek) inputStream.Position = 0;

            using var img = await Image.LoadAsync(inputStream);

            img.Metadata.ExifProfile = null;

            if (img.Width > MaxImageWidth)
            {
                img.Mutate(x => x.Resize(MaxImageWidth, 0));
            }

            if (extension == ".jpg" || extension == ".jpeg")
            {
                await img.SaveAsJpegAsync(outputStream, new JpegEncoder { Quality = JpegQuality });
            }
            else if (extension == ".png")
            {
                await img.SaveAsPngAsync(outputStream, new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression });
            }
            else
            {
                await img.SaveAsJpegAsync(outputStream, new JpegEncoder { Quality = JpegQuality });
            }
        }

        private void ValidateSize(IFormFile file)
        {
            if (file.Length > _maxSize)
                throw new InvalidOperationException("Plik jest zbyt duży.");
        }

        private void ValidateExtension(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLower();

            if (!_allowedExtensions.Contains(ext))
                throw new InvalidOperationException("Niedozwolone rozszerzenie pliku.");
        }

        private static async Task ValidateMagicNumbers(IFormFile file)
        {
            byte[] buffer = new byte[MagicNumberBufferSize];
            await using var stream = file.OpenReadStream();

            int bytesRead = await stream.ReadAsync(buffer);

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            // JPEG FF D8
            if (bytesRead >= 2 && buffer[0] == 0xFF && buffer[1] == 0xD8) return;

            // PNG header
            if (bytesRead >= 8 && new ReadOnlySpan<byte>(buffer, 0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 })) return;

            // WebP RIFF....WEBP
            if (bytesRead >= 12 && new ReadOnlySpan<byte>(buffer, 0, 4).SequenceEqual("RIFF"u8) && new ReadOnlySpan<byte>(buffer, 8, 4).SequenceEqual("WEBP"u8)) return;

            // PDF header: %PDF (25 50 44 46)
            if (bytesRead >= 4 && new ReadOnlySpan<byte>(buffer, 0, 4).SequenceEqual("%PDF"u8)) return;

            throw new InvalidOperationException($"Plik nie jest prawidłowym obrazem lub PDF (nieprawidłowy nagłówek).");
        }
    }
}
