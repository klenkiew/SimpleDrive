using FileService.Model;

namespace FileService.Events
{
    public class FileSharesChangedMessage
    {
        public File File { get; }
        public string UserId { get; }

        public FileSharesChangedMessage(File file, string userId)
        {
            File = file;
            UserId = userId;
        }
    }
}