namespace AuthenticationService.Requests
{
    public class ConfirmEmailChangeRequest
    {
        public string Email { get; }
        public string Token { get; }

        public ConfirmEmailChangeRequest(string email, string token)
        {
            Email = email;
            Token = token;
        }
    }
}