using System.Collections.Generic;
using FileService.Dto;

namespace FileService.Queries
{
    public class FindUsersBySharedFileQuery : IQuery<IEnumerable<UserDto>>
    {
        public string FileId { get; set; }
    }
}