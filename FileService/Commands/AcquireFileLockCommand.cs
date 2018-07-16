namespace FileService.Commands
{
    public class AcquireFileLockCommand
    {
        public string FileId { get; }

        public AcquireFileLockCommand(string fileId)
        {
            FileId = fileId;
        }
    }
}