using FluentValidation;

namespace FileService.Requests
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(r => r.Email).NotNull().EmailAddress();
            RuleFor(r => r.Username).NotNull().MinimumLength(2).MaximumLength(100);
            RuleFor(r => r.Password).NotNull().MinimumLength(6).MaximumLength(100);
            RuleFor(r => r.PasswordConfirmation).NotNull().MinimumLength(6).MaximumLength(100).Equal(r => r.Password);
        }
    }
}