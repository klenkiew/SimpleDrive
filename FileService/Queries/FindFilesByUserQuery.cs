using System.Collections.Generic;
using FileService.Dto;

namespace FileService.Queries
{
    public class FindFilesByUserQuery : IQuery<IEnumerable<FileDto>>
    {
        public string UserId { get; set; }
    }
}