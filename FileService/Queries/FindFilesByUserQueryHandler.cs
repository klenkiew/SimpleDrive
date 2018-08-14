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
            const string sql = "GetAvailableFilesByUser";

            IEnumerable<FileDto> files = dbConnection.Query<FileDto, UserDto, FileDto>(
                sql,
                (file, user) =>
                {
                    file.Owner = user;
                    return file;
                },
                new {UserId = query.UserId}, 
                commandType: CommandType.StoredProcedure);
            
            return files;
        }
    }
}