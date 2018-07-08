using FluentValidation;

namespace FileService.Commands.Validators
{
    public class DeleteFileCommandValidator : AbstractValidator<DeleteFileCommand>
    {
        public DeleteFileCommandValidator()
        {
            RuleFor(cmd => cmd.FileId).NotNull().NotEmpty();
        }
    }
}