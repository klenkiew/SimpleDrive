namespace AuthenticationService.Configuration
{
    public class EmailClientConfiguration
    {
        public string Address { get; set; }
        public string Username { get; set; }
        public string Password  { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSsl { get; set; }
    }
}