namespace FleetlyBackend.Services.FileService
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string pathPrefix);
        Task DeleteFileAsync(string filePath);
    }
}
