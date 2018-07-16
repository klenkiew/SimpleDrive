namespace AuthenticationService.Requests
{
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; }
        public string NewPassword { get; }
        public string PasswordConfirmation { get; }

        public ChangePasswordRequest(string currentPassword, string newPassword, string passwordConfirmation)
        {
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
            PasswordConfirmation = passwordConfirmation;
        }
    }
}