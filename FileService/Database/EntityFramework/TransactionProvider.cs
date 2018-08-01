using Microsoft.EntityFrameworkCore.Storage;

namespace FileService.Database.EntityFramework
{
    internal class TransactionProvider : ITransactionProvider
    {
        private readonly FileDbContext dbContext;

        public TransactionProvider(FileDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public ITransaction BeginTransaction()
        {
            return new Transaction(dbContext.Database.BeginTransaction());
        }
    }
    
    internal class Transaction : ITransaction
    {
        private readonly IDbContextTransaction transaction;

        public Transaction(IDbContextTransaction transaction)
        {
            this.transaction = transaction;
        }

        public void Commit()
        {
            transaction.Commit();
        }

        public void Rollback()
        {
            transaction.Rollback();
        }

        public void Dispose()
        {
            transaction?.Dispose();
        }
    }
}