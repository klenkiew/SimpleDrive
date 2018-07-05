using FluentValidation;

namespace AuthenticationService.Requests
{
    public class ChangeEmailRequestValidator : AbstractValidator<ChangeEmailRequest>
    {
        public ChangeEmailRequestValidator()
        {
            RuleFor(r => r.Email).NotNull().EmailAddress();
        }
    }
}