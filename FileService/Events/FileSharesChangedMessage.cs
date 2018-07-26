using FileService.Model;

namespace FileService.Events
{
    public class FileSharesChangedMessage
    {
        public File File { get; }
        public FileShare Share { get; }

        public FileSharesChangedMessage(File file, FileShare share)
        {
            File = file;
            Share = share;
        }
    }
}