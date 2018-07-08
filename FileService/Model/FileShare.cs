namespace FileService.Model
{
    public class FileShare
    {
        public virtual string FileId { get; set; }
        public virtual File File { get; set; }
        public virtual string UserId { get; set; }
        public virtual User User { get; set; }
    }
}