using System.Data;
using System.IO;
using System.Linq;
using Dapper;
using FileService.Dto;
using FileService.Services;
using File = FileService.Model.File;

namespace FileService.Queries
{
    public class GetFileContentQueryHandler : IQueryHandler<GetFileContentQuery, FileContentDto>
    {
        private readonly IDbConnection dbConnection;
        private readonly IFileStorage fileStorage;
        private readonly IMapper<FileDto, File> mapper;

        public GetFileContentQueryHandler(
            IDbConnection dbConnection, 
            IFileStorage fileStorage, 
            IMapper<FileDto, File> mapper)
        {
            this.dbConnection = dbConnection;
            this.fileStorage = fileStorage;
            this.mapper = mapper;
        }

        public FileContentDto Handle(GetFileContentQuery query)
        {
            const string sql =
                "SELECT " +
                "f.[Id], f.[FileName], f.[Description], f.[Size], f.[MimeType], f.[DateCreated], f.[DateModified], " +
                "u.[Id], u.[Username] " +
                "FROM [Files] f " +
                "LEFT JOIN [Users] u ON f.[OwnerId] = u.[Id] " +
                "WHERE f.[Id] = @FileId";

            FileDto fileDto = dbConnection.Query<FileDto, UserDto, FileDto>(
                sql,
                (file, user) =>
                {
                    file.Owner = user;
                    return file;
                },
                new {FileId = query.FileId}).FirstOrDefault();

            Stream content = fileStorage.ReadFile(mapper.Map(fileDto)).Result;
            return new FileContentDto(fileDto.MimeType, content);
        }
    }
}