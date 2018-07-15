namespace FileService.Dto
{
    public class FileContentChangedMessage
    {
        public string FileId { get; }
        public FileContentChange ContentChange { get; }
        public CaretChange CaretChange { get; }

        public FileContentChangedMessage(string fileId, FileContentChange contentChange, CaretChange caretChange)
        {
            FileId = fileId;
            ContentChange = contentChange;
            CaretChange = caretChange;
        }
    }
    
    public class CaretChange
    {
        public long NewSelectionStart { get; }
        public long NewSelectionEnd { get; }

        public CaretChange(long newSelectionStart, long newSelectionEnd)
        {
            NewSelectionStart = newSelectionStart;
            NewSelectionEnd = newSelectionEnd;
        }
    }

    public class FileContentChange
    {
        public string NewContent { get; }

        public FileContentChange(string newContent)
        {
            NewContent = newContent;
        }
    }
}