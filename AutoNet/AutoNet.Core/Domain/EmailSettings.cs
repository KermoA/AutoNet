namespace AutoNet.Core.Domain
{
    public class EmailSettings
    {
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
        public string SmtpHost { get; set; }
        public string SmtpPort { get; set; }
        public bool EnableSsl { get; set; }
    }
}
