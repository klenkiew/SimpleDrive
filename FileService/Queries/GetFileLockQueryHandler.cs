using System.Data;
using System.Linq;
using Dapper;
using FileService.Dto;
using FileService.Model;
using FileService.Services;

namespace FileService.Queries
{
    public class GetFileLockQueryHandler : IQueryHandler<GetFileLockQuery, FileLockDto>
    {
        private readonly IDbConnection dbConnection;
        private readonly IFileLockingService fileLockingService;
        private readonly IMapper<FileDto, File> mapper;

        public GetFileLockQueryHandler(
            IDbConnection dbConnection, 
            IFileLockingService fileLockingService, 
            IMapper<FileDto, File> mapper)
        {
            this.dbConnection = dbConnection;
            this.fileLockingService = fileLockingService;
            this.mapper = mapper;
        }

        public FileLockDto Handle(GetFileLockQuery query)
        {
            const string sql =
                "SELECT f.\"Id\", f.\"FileName\", f.\"Description\", f.\"Size\", f.\"MimeType\", f.\"DateCreated\", f.\"DateModified\" " +
                "FROM \"Files\" f " +
                "WHERE f.\"Id\" = @FileId";

            FileDto fileDto = dbConnection.Query<FileDto>(sql, new {FileId = query.FileId}).FirstOrDefault();

            UserDto lockOwner = fileLockingService.GetLockOwner(mapper.Map(fileDto));
            return FileLockDto.ForUser(lockOwner);
        }
    }
}