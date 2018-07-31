using System;
using Microsoft.EntityFrameworkCore.Storage;

namespace FileService.Database
{
    public interface ITransactionProvider
    {
        ITransaction BeginTransaction();
    }

    public interface ITransaction : IDisposable
    {
        void Commit();
        void Rollback();
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

    public static class TransactionProviderExtensions
    {
        public static void ExecuteInTransaction(this ITransactionProvider transactionProvider, Action<ITransaction> action)
        {
            using (ITransaction transaction = transactionProvider.BeginTransaction())
            {
                action(transaction); 
                transaction.Commit();                
            }
        }
        
        public static void ExecuteInTransaction(this ITransactionProvider transactionProvider, Action action)
        {
            using (ITransaction transaction = transactionProvider.BeginTransaction())
            {
                action(); 
                transaction.Commit();                
            }
        }
    }
}