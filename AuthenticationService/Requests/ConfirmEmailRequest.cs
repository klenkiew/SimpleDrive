namespace AuthenticationService.Requests
{
    public class ConfirmEmailRequest
    {
        public string UserId { get; }
        public string Token { get; }

        public ConfirmEmailRequest(string userId, string token)
        {
            UserId = userId;
            Token = token;
        }
    }
}