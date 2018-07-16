namespace FileService.Commands
{
    public class RemoveFileLockCommand
    {
        public string FileId { get; }

        public RemoveFileLockCommand(string fileId)
        {
            FileId = fileId;
        }
    }
}