using FileService.Commands;

namespace FileService.Requests
{
    public class AddShareRequestCommandAdapter : ICommandHandler<ShareFileRequest>
    {
        private readonly ICommandHandler<ShareFileCommand> adaptee;

        public AddShareRequestCommandAdapter(ICommandHandler<ShareFileCommand> adaptee)
        {
            this.adaptee = adaptee;
        }

        public void Handle(ShareFileRequest request)
        {
            var command = MapRequestToCommand(request);
            adaptee.Handle(command);
        }

        private ShareFileCommand MapRequestToCommand(ShareFileRequest request)
        {
            if (request == null)
                return null;

            return new ShareFileCommand()
            {
                FileId = request.FileId,
                ShareWithUserId = request.ShareWithUserId
            };
        }
    }
}