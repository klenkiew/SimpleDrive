using FileService.Database;

namespace FileService.Commands.Decorators
{
    internal class PostCommitCommandHandlerDecorator<T> : ICommandHandler<T>
    {
        private readonly ICommandHandler<T> decorated;
        private readonly PostCommitRegistratorImpl registrator;
 
        public PostCommitCommandHandlerDecorator(
            ICommandHandler<T> decorated, PostCommitRegistratorImpl registrator)
        {
            this.decorated = decorated;
            this.registrator = registrator;
        }
 
        public void Handle(T command)
        {
            try
            {
                decorated.Handle(command);
                registrator.ExecuteActions();
            }
            finally
            {
                registrator.Reset();
            }
        }
    }
}