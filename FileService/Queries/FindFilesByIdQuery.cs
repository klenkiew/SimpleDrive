using FileService.Model;

namespace FileService.Queries
{
    public class FindFileByIdQuery : IQuery<File>
    {
        public string FileId { get; set; }
    }
}