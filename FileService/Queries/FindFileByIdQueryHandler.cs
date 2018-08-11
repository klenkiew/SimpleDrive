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
            const string sql =
                "SELECT " +
                "f.\"Id\", f.\"FileName\", f.\"Description\", f.\"Size\", f.\"MimeType\", f.\"DateCreated\", f.\"DateModified\", " +
                "u.\"Id\", u.\"Username\" " +
                "FROM \"Files\" f " +
                "LEFT JOIN \"Users\" u ON f.\"OwnerId\" = u.\"Id\" " +
                "WHERE f.\"Id\" = @FileId";

            FileDto fileDto = dbConnection.Query<FileDto, UserDto, FileDto>(
                sql,
                (file, user) =>
                {
                    file.Owner = user;
                    return file;
                },
                new {FileId = query.FileId}).FirstOrDefault();
            
            return fileDto;
        }
    }
}