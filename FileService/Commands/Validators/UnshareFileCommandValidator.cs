using FluentValidation;

namespace FileService.Commands.Validators
{
    public class UnshareFileCommandValidator : AbstractValidator<UnshareFileCommand>
    {
        public UnshareFileCommandValidator()
        {
            RuleFor(cmd => cmd.FileId).NotNull().NotEmpty();
            RuleFor(cmd => cmd.UserId).NotNull().NotEmpty();
        }
    }
}