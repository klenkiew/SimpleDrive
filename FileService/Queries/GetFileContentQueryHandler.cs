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

            Stream content = fileStorage.ReadFile(mapper.Map(fileDto)).Result;
            return new FileContentDto(fileDto.MimeType, content);
        }
    }
}