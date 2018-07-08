using FluentValidation;

namespace FileService.Commands.Validators
{
    public class AddFileCommandValidator : AbstractValidator<AddFileCommand>
    {
        public AddFileCommandValidator()
        {
            RuleFor(cmd => cmd.FileName).NotNull().NotEmpty();
            RuleFor(cmd => cmd.Content).NotNull();
        }
    }
}