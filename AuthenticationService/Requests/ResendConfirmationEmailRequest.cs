namespace AuthenticationService.Requests
{
    public class ResendConfirmationEmailRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}