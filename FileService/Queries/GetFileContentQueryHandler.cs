using System;
using System.IO;
using FileService.Services;

namespace FileService.Queries
{
    public class GetFileContentQueryHandler : IQueryHandler<GetFileContentQuery, Stream>
    {
        private readonly IFileStorage fileStorage;

        public GetFileContentQueryHandler(IFileStorage fileStorage)
        {
            this.fileStorage = fileStorage;
        }

        public Stream Handle(GetFileContentQuery query)
        {
            return fileStorage.ReadFile(query.Owner, query.FileId).Result;
        }
    }
}