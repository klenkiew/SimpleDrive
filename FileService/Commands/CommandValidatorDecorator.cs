using FluentValidation;

namespace FileService.Commands
{
    internal class CommandValidatorDecorator<TCommand> : ICommandHandler<TCommand>
    {
        private readonly ICommandHandler<TCommand> decorated;
        private readonly AbstractValidator<TCommand> validator;

        public CommandValidatorDecorator(ICommandHandler<TCommand> decorated, AbstractValidator<TCommand> validator)
        {
            this.validator = validator;
            this.decorated = decorated;
        }

        public void Handle(TCommand command)
        {
            validator.ValidateAndThrow(command);
            decorated.Handle(command);
        }
    }
}