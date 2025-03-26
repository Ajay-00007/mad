using Microsoft.AspNetCore.Http;
using StudentManagementSystem.Configuration;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string? customFileName = null);
        Task<byte[]> GetFileAsync(string fileName);
        Task DeleteFileAsync(string fileName);
        bool IsValidFileType(IFormFile file);
        bool IsValidFileSize(IFormFile file);
        string GetUniqueFileName(string fileName);
        string GetFileExtension(string fileName);
        string GetContentType(string fileName);
    }

    public class FileService : IFileService
    {
        private readonly IConfigurationService _config;
        private readonly ILoggingService _logger;
        private readonly string _uploadPath;

        public FileService(IConfigurationService config, ILoggingService logger, IWebHostEnvironment environment)
        {
            _config = config;
            _logger = logger;
            _uploadPath = Path.Combine(environment.WebRootPath, _config.GetFileStorage().UploadDirectory);

            // Ensure upload directory exists
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file, string? customFileName = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File is empty or null");
                }

                if (!IsValidFileType(file))
                {
                    throw new AppException(
                        string.Format(Constants.ErrorMessages.InvalidFileType, 
                        string.Join(", ", _config.GetFileStorage().AllowedFileTypes)));
                }

                if (!IsValidFileSize(file))
                {
                    throw new AppException(
                        string.Format(Constants.ErrorMessages.FileSizeExceeded, 
                        _config.GetFileStorage().MaxFileSizeBytes / (1024 * 1024)));
                }

                var fileName = customFileName ?? GetUniqueFileName(file.FileName);
                var filePath = Path.Combine(_uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("File saved successfully: {FileName}", fileName);
                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file: {FileName}", file.FileName);
                throw;
            }
        }

        public async Task<byte[]> GetFileAsync(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_uploadPath, fileName);
                if (!File.Exists(filePath))
                {
                    throw new NotFoundException($"File not found: {fileName}");
                }

                return await File.ReadAllBytesAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving file: {FileName}", fileName);
                throw;
            }
        }

        public async Task DeleteFileAsync(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_uploadPath, fileName);
                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                    _logger.LogInformation("File deleted successfully: {FileName}", fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FileName}", fileName);
                throw;
            }
        }

        public bool IsValidFileType(IFormFile file)
        {
            if (file == null)
                return false;

            var extension = GetFileExtension(file.FileName);
            return _config.GetFileStorage().AllowedFileTypes.Contains(extension.ToLower());
        }

        public bool IsValidFileSize(IFormFile file)
        {
            return file != null && file.Length <= _config.GetFileStorage().MaxFileSizeBytes;
        }

        public string GetUniqueFileName(string fileName)
        {
            var extension = GetFileExtension(fileName);
            var baseFileName = Path.GetFileNameWithoutExtension(fileName);
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var random = Path.GetRandomFileName().Replace(".", "").Substring(0, 4);
            
            return $"{baseFileName}_{timestamp}_{random}{extension}";
        }

        public string GetFileExtension(string fileName)
        {
            return Path.GetExtension(fileName).ToLowerInvariant();
        }

        public string GetContentType(string fileName)
        {
            var extension = GetFileExtension(fileName).ToLower();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }

        private string SanitizeFileName(string fileName)
        {
            // Remove invalid characters
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitizedName = string.Join("_", fileName.Split(invalidChars));

            // Replace spaces with underscores
            sanitizedName = sanitizedName.Replace(" ", "_");

            // Remove multiple consecutive underscores
            sanitizedName = string.Join("_", sanitizedName.Split(new[] { '_' }, 
                StringSplitOptions.RemoveEmptyEntries));

            return sanitizedName;
        }
    }
}