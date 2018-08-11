using System.Collections.Generic;
using System.Data;
using Dapper;
using FileService.Dto;

namespace FileService.Queries
{
    public class FindFilesByUserQueryHandler : IQueryHandler<FindFilesByUserQuery, IEnumerable<FileDto>>
    {
        private readonly IDbConnection dbConnection;

        public FindFilesByUserQueryHandler(IDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public IEnumerable<FileDto> Handle(FindFilesByUserQuery query)
        {
            const string sql =
                "SELECT " +
                "f.\"Id\", f.\"FileName\", f.\"Description\", f.\"Size\", f.\"MimeType\", f.\"DateCreated\", f.\"DateModified\", " +
                "u.\"Id\", u.\"Username\" " +
                "FROM \"Files\" f " +
                "INNER JOIN \"Users\" u ON f.\"OwnerId\" = u.\"Id\" " +
                "LEFT JOIN \"FileShare\" s ON s.\"FileId\" = f.\"Id\" " +
                "WHERE f.\"OwnerId\" = @UserId OR s.\"UserId\" = @UserId";

            IEnumerable<FileDto> files = dbConnection.Query<FileDto, UserDto, FileDto>(
                sql,
                (file, user) =>
                {
                    file.Owner = user;
                    return file;
                },
                new {UserId = query.UserId});
            return files;
        }
    }
}