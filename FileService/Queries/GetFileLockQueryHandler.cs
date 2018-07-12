using System.Linq;
using FileService.Database;
using FileService.Dto;
using FileService.Exceptions;
using FileService.Services;

namespace FileService.Queries
{
    public class GetFileLockQueryHandler : IQueryHandler<GetFileLockQuery, FileLockDto>
    {
        private readonly IFileLockingService fileLockingService;
        private readonly FileDbContext dbContext;

        public GetFileLockQueryHandler(IFileLockingService fileLockingService, FileDbContext dbContext)
        {
            this.fileLockingService = fileLockingService;
            this.dbContext = dbContext;
        }

        public FileLockDto Handle(GetFileLockQuery query)
        {
            var file = dbContext.Files.FirstOrDefault(f => f.Id == query.FileId);

            if (file == null)
                throw new NotFoundException($"A file with id {query.FileId} doesn't exist in the database.");

            var lockOwner = fileLockingService.GetLockOwner(file);
            return new FileLockDto(lockOwner != null, lockOwner);
        }
    }
}