using ProductDataAccess.Models.Response;

namespace ProductAPI.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName);
        ValidationResult ValidateImage(IFormFile file);
    }
}
