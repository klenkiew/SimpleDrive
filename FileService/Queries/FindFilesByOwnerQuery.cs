using System.Collections;
using System.Collections.Generic;
using FileService.Model;

namespace FileService.Queries
{
    public class FindFilesByOwnerQuery : IQuery<IEnumerable<File>>
    {
        public string OwnerId { get; set; }
    }
}