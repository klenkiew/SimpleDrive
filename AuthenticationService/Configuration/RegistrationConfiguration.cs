namespace AuthenticationService.Configuration
{
    public class RegistrationConfiguration
    {
        public bool RequireEmailConfirmation { get; set; }
        public string CallbackUrl { get; set; }
        public EmailClientConfiguration EmailClient { get; set; }
    }

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