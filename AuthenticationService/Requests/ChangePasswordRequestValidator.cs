using AuthenticationService.Requests;
using FluentValidation;

namespace FileService.Requests
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(r => r.CurrentPassword).NotNull().MinimumLength(6).MaximumLength(100);
            RuleFor(r => r.NewPassword).NotNull().MinimumLength(6).MaximumLength(100);
            RuleFor(r => r.PasswordConfirmation).NotNull().MinimumLength(6).MaximumLength(100).Equal(r => r.NewPassword);
        }
    }
}