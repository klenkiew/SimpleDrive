namespace AuthenticationService.Validation
{
    public class BasicError
    {
        public string Message { get; }

        public BasicError(string message)
        {
            Message = message;
        }
    }
}