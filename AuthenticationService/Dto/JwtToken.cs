namespace AuthenticationService.Dto
{
    public class JwtToken
    {
        public string Token { get; }

        public JwtToken(string token)
        {
            Token = token;
        }
    }
}