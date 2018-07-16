namespace AuthenticationService.Requests
{
    public class ChangeEmailRequest
    {
        public string Email { get; }
        public string Password { get; }

        public ChangeEmailRequest(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}