namespace FileService.Commands
{
    public class DeleteFileCommand
    {
        public string FileId { get; }

        public DeleteFileCommand(string fileId)
        {
            FileId = fileId;
        }
    }
}