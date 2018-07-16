namespace FileService.Dto
{
    public class CurrentLockNotificationsSubscriptionMessage
    {
        public string FileId { get; }

        public CurrentLockNotificationsSubscriptionMessage(string fileId)
        {
            FileId = fileId;
        }
    }
}