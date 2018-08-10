using FileService.Database;

namespace FileService.Commands.Decorators
{
    internal class TransactionCommandHandlerDecorator<T> : ICommandHandler<T>
    {
        private readonly ICommandHandler<T> decorated;
        private readonly IUnitOfWork unitOfWork;
        
        public TransactionCommandHandlerDecorator(ICommandHandler<T> decorated, IUnitOfWork unitOfWork)
        {
            this.decorated = decorated;
            this.unitOfWork = unitOfWork;
        }
 
        public void Handle(T command)
        {
            try
            {
                decorated.Handle(command);
                unitOfWork.Commit();
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }
        }
    }
}