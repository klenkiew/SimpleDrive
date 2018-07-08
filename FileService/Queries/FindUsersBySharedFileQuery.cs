using System.Collections.Generic;
using FileService.Model;

namespace FileService.Queries
{
    public class FindUsersBySharedFileQuery : IQuery<IEnumerable<User>>
    {
        public string FileId { get; set; }
    }
}