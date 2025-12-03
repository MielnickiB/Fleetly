namespace FleetlyBackend.Services.FileService
{
    public class FileUploadOptions
    {
        public string RootPath { get; set; } = "Uploads";
        public string ImageFolder { get; set; } = "Images";
        public long MaxFileSize { get; set; } = 5 * 1024 * 1024;
        public string[] AllowedExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".webp"];
    }
}
