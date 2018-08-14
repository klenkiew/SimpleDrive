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
            const string sql = "GetUsersBySharedFile";

            return dbConnection
                .Query<UserDto>(sql, new {FileId = query.FileId}, commandType: CommandType.StoredProcedure);
        }
    }
}