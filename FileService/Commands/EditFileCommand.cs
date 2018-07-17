namespace FileService.Commands
{
    public class EditFileCommand
    {
        public string FileId { get; }
        public string FileName { get; }
        public string Description { get; }

        public EditFileCommand(string fileId, string fileName, string description)
        {
            FileId = fileId;
            FileName = fileName;
            Description = description;
        }
    }
}