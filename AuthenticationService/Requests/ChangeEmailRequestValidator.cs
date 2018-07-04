using FileService.Controllers;
using FluentValidation;

namespace FileService.Requests
{
    public class ChangeEmailRequestValidator : AbstractValidator<ChangeEmailRequest>
    {
        public ChangeEmailRequestValidator()
        {
            RuleFor(r => r.Email).NotNull().EmailAddress();
        }
    }
}