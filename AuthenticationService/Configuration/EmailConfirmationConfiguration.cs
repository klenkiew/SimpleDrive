namespace AuthenticationService.Configuration
{
    public class EmailConfirmationConfiguration
    {
        public bool RequireEmailConfirmation { get; set; }
        public string ConfirmEmailCallbackUrl { get; set; }
        public string ConfirmEmailChangeCallbackUrl { get; set; }
    }
}