using FluentValidation;

namespace FileService.Commands
{
    internal class CommandValidator<TCommand> : ICommandHandler<TCommand>
    {
        private readonly AbstractValidator<TCommand> validator;

        public CommandValidator(AbstractValidator<TCommand> validator)
        {
            this.validator = validator;
        }

        public void Handle(TCommand command)
        {
            validator.ValidateAndThrow(command);
        }
    }
}