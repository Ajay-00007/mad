namespace StudentManagementSystem.Configuration
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = "smtp.example.com";
        public int SmtpPort { get; set; } = 587;
        public string SmtpUsername { get; set; } = "saieleuri@gmail.com";
        public string SmtpPassword { get; set; } = "gfzt zbdv jofx kbql";
        public string FromEmail { get; set; } = "studentmanagement@gmail.com";
        public string FromName { get; set; } = "studentmanagementteam";
        public bool EnableSsl { get; set; } = true;
        public int Timeout { get; set; } = 30000; // 30 seconds
    }
}