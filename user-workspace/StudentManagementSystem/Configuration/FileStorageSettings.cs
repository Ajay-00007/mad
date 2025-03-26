namespace StudentManagementSystem.Configuration
{
    public class FileStorageSettings
    {
        public string UploadDirectory { get; set; } = "uploads";
        public string[] AllowedFileTypes { get; set; } = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
        public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB
        public bool PreserveFileNames { get; set; } = false;
        public bool CreateYearMonthFolders { get; set; } = true;
        public string TempDirectory { get; set; } = "temp";
        public int RetentionDays { get; set; } = 30;
        public bool CompressImages { get; set; } = true;
        public int ImageQuality { get; set; } = 80;
        public int MaxImageWidth { get; set; } = 1920;
        public int MaxImageHeight { get; set; } = 1080;
        public string[] AllowedImageTypes { get; set; } = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        public string[] AllowedDocumentTypes { get; set; } = new[] { ".pdf", ".doc", ".docx", ".txt" };
        public string DefaultUploadPermissions { get; set; } = "private";
        public bool EnableVersioning { get; set; } = true;
        public int MaxVersions { get; set; } = 5;
        public string BasePath { get; set; }
        public long MaxFileSizeBytes { get; set; }
    }
}