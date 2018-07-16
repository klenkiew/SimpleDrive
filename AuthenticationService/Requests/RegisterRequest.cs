namespace AuthenticationService.Requests
{
    public class RegisterRequest
    {
        public string Email { get; }
        public string Username { get; }
        public string Password { get; }
        public string PasswordConfirmation { get; }

        public RegisterRequest(string email, string username, string password, string passwordConfirmation)
        {
            Email = email;
            Username = username;
            Password = password;
            PasswordConfirmation = passwordConfirmation;
        }
    }
}