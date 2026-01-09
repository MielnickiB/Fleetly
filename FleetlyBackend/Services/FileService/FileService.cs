using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace FleetlyBackend.Services.FileService
{
    public class FileService(IOptions<FileUploadOptions> options) : IFileService
    {
        private readonly string _rootPath = options.Value.RootPath;
        private readonly long _maxSize = options.Value.MaxFileSize;
        private readonly string[] _allowedExtensions = options.Value.AllowedExtensions;

        private const int MaxImageWidth = 2000;
        private const int JpegQuality = 85;
        private const int MagicNumberBufferSize = 12;

        public async Task<string> SaveFileAsync(IFormFile file, string pathPrefix)
        {
            ValidateSize(file);
            ValidateExtension(file);
            await ValidateMagicNumbers(file);

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName).ToLower()}";
            var folderPath = Path.Combine(_rootPath, pathPrefix);

            var fullFilePath = Path.Combine(folderPath, uniqueFileName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var ext = Path.GetExtension(file.FileName).ToLower();

            if (ext == ".pdf")
            {
                await using var stream = new FileStream(fullFilePath, FileMode.Create);
                await file.CopyToAsync(stream);
            }
            else
            {
                await SaveImageWithCompression(file, fullFilePath);
            }

            return Path.Combine(pathPrefix, uniqueFileName).Replace("\\", "/");
        }

        public Task DeleteFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_rootPath, filePath);

            var safePath = Path.GetFullPath(fullPath);
            if (!safePath.StartsWith(Path.GetFullPath(_rootPath)))
                throw new InvalidOperationException("Niepoprawna ścieżka pliku.");
            if (File.Exists(safePath))
                File.Delete(safePath);

            return Task.CompletedTask;
        }

        private static async Task SaveImageWithCompression(IFormFile file, string filePath)
        {
            await using var stream = file.OpenReadStream();

            using var img = await Image.LoadAsync(stream);

            img.Metadata.ExifProfile = null;

            if (img.Width > MaxImageWidth)
            {
                img.Mutate(x => x.Resize(MaxImageWidth, 0));
            }

            await img.SaveAsJpegAsync(filePath, new JpegEncoder
            {
                Quality = JpegQuality
            });
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

            // JPEG FF D8
            if (bytesRead >= 2 && buffer[0] == 0xFF && buffer[1] == 0xD8) return;

            // PNG header
            if (bytesRead >= 8 && new ReadOnlySpan<byte>(buffer, 0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 })) return;

            // WebP RIFF....WEBP
            if (bytesRead >= 12 && new ReadOnlySpan<byte>(buffer, 0, 4).SequenceEqual("RIFF"u8) && new ReadOnlySpan<byte>(buffer, 8, 4).SequenceEqual("WEBP"u8)) return;

            // PDF header: %PDF (25 50 44 46)
            if (bytesRead >= 4 && new ReadOnlySpan<byte>(buffer, 0, 4).SequenceEqual("%PDF"u8)) return;

            throw new InvalidOperationException("Plik nie jest prawidłowym obrazem.");
        }
    }
}
