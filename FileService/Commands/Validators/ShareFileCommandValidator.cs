using FluentValidation;

namespace FileService.Commands.Validators
{
    public class ShareFileCommandValidator : AbstractValidator<ShareFileCommand>
    {
        public ShareFileCommandValidator()
        {
            RuleFor(cmd => cmd.FileId).NotNull().NotEmpty();
            RuleFor(cmd => cmd.ShareWithUserId).NotNull().NotEmpty();
        }
    }
}