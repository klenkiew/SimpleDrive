using System;

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