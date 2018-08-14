using System.Data;
using System.Linq;
using Dapper;
using FileService.Dto;

namespace FileService.Queries
{
    public class FindFileByIdQueryHandler : IQueryHandler<FindFileByIdQuery, FileDto>
    {
        private readonly IDbConnection dbConnection;

        public FindFileByIdQueryHandler(IDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public FileDto Handle(FindFileByIdQuery query)
        {
            const string sql = "GetFileWithOwner";

            FileDto fileDto = dbConnection.Query<FileDto, UserDto, FileDto>(
                sql,
                (file, user) =>
                {
                    file.Owner = user;
                    return file;
                },
                new {FileId = query.FileId},
                commandType: CommandType.StoredProcedure).FirstOrDefault();
            
            return fileDto;
        }
    }
}