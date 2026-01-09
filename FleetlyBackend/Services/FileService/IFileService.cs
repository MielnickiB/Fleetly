namespace FleetlyBackend.Services.FileService
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file);
        Task DeleteFileAsync(string filePath);
    }
}
