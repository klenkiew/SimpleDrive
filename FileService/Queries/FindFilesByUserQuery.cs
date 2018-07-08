using System.Collections.Generic;
using FileService.Model;

namespace FileService.Queries
{
    public class FindFilesByUserQuery : IQuery<IEnumerable<File>>
    {
        public string UserId { get; set; }
    }
}