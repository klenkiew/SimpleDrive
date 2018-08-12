using System.Collections.Generic;
using System.Data;
using Dapper;
using FileService.Dto;

namespace FileService.Queries
{
    public class FindUsersBySharedFileQueryHandler : IQueryHandler<FindUsersBySharedFileQuery, IEnumerable<UserDto>>
    {
        private readonly IDbConnection dbConnection;

        public FindUsersBySharedFileQueryHandler(IDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public IEnumerable<UserDto> Handle(FindUsersBySharedFileQuery query)
        {
            const string sql =
                "SELECT u.[Id], u.[Username] " +
                "FROM [Files] f " +
                "INNER JOIN [FileShare] s ON s.[FileId] = f.[Id] " +
                "INNER JOIN [Users] u ON s.[UserId] = u.[Id] " +
                "WHERE f.[Id] = @FileId";

            return dbConnection.Query<UserDto>(sql, new {FileId = query.FileId});
        }
    }
}