namespace FleetlyBackend.Services.FileService
{
    public interface IFileService
    {
        Task<string> SaveImageAsync(IFormFile file);
        Task DeleteImageAsync(string filePath);
    }
}
