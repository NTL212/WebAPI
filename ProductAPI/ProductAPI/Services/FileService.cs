namespace ProductAPI.Services
{
    public class FileService : IFileService
    {
        private readonly string _basePath;

        public FileService(IConfiguration configuration)
        {
            _basePath = configuration["FileStorage:BasePath"]; // Đường dẫn được cấu hình trong appsettings.json
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            var folderPath = Path.Combine(_basePath, folderName);
            Directory.CreateDirectory(folderPath); // Tạo thư mục nếu chưa tồn tại

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        public ValidationResult ValidateImage(IFormFile file)
        {
            if (!file.ContentType.StartsWith("image/"))
            {
                return new ValidationResult(false, "The uploaded file is not a valid image.");
            }

            if (file.Length > 5 * 1024 * 1024) // 5MB
            {
                return new ValidationResult(false, "Image size must be less than 5MB.");
            }

            return new ValidationResult(true);
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; }
        public string ErrorMessage { get; }

        public ValidationResult(bool isValid, string errorMessage = "")
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }
    }

}
