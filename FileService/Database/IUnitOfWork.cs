using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FileService.Database
{
    public interface IUnitOfWork
    {
        void Commit();
        void Rollback();
    }

    internal class UnitOfWork : IUnitOfWork
    {
        private readonly FileDbContext dbContext;

        public UnitOfWork(FileDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Commit()
        {
            dbContext.SaveChanges();
        }

        public void Rollback()
        {
            // TODO an empty implementation is ok as long as the context isn't used after a 'rollback'
            // in which case the changes made before the rollback would be persisted on the next SaveChanges call
            // consider manually setting all tracked entities state to unchanged/detached
            // - possible performance penalty
        }
    }
}