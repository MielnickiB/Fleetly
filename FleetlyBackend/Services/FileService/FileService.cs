using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace FleetlyBackend.Services.FileService
{
    public class FileService : IFileService
    {
        private readonly string _root;
        private readonly string _imageFolder;
        private readonly long _maxSize;
        private readonly string[] _allowedExtensions;

        private const int MaxImageWidth = 2000;
        private const int JpegQuality = 85;
        private const int MagicNumberBufferSize = 12;

        public FileService(IOptions<FileUploadOptions> options)
        {
            var opt = options.Value;

            _root = opt.RootPath;
            _imageFolder = Path.Combine(_root, opt.ImageFolder);
            _maxSize = opt.MaxFileSize;
            _allowedExtensions = opt.AllowedExtensions;

            if (!Directory.Exists(_imageFolder))
                Directory.CreateDirectory(_imageFolder);
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            ValidateSize(file);
            ValidateExtension(file);
            await ValidateMagicNumbers(file);

            var fileName = $"{Guid.NewGuid()}.jpg";
            var filePath = Path.Combine(_imageFolder, fileName);

            await using var stream = file.OpenReadStream();
            using var img = await Image.LoadAsync(stream);

            // EXIF cleanup + resize
            img.Metadata.ExifProfile = null;

            if (img.Width > MaxImageWidth)
            {
                img.Mutate(x => x.Resize(MaxImageWidth, 0));
            }

            await img.SaveAsJpegAsync(filePath, new JpegEncoder
            {
                Quality = JpegQuality
            });

            return fileName;
        }

        public Task DeleteFileAsync(string fileName)
        {
            var fullPath = Path.Combine(_imageFolder, fileName);

            var safePath = Path.GetFullPath(fullPath);
            if (!safePath.StartsWith(Path.GetFullPath(_imageFolder)))
                throw new InvalidOperationException("Niepoprawna ścieżka pliku.");
            if (File.Exists(safePath))
                File.Delete(safePath);

            return Task.CompletedTask;
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

            throw new InvalidOperationException("Plik nie jest prawidłowym obrazem.");
        }
    }
}
