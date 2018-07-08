using FileService.Commands;

namespace FileService.Requests
{
    public class AddFileRequestCommandAdapter : ICommandHandler<AddFileRequest>
    {
        private readonly ICommandHandler<AddFileCommand> adaptee;

        public AddFileRequestCommandAdapter(ICommandHandler<AddFileCommand> adaptee)
        {
            this.adaptee = adaptee;
        }

        public void Handle(AddFileRequest request)
        {
            var command = MapRequestToCommand(request);
            using (command.Content = request.File?.OpenReadStream())
                adaptee.Handle(command);
        }

        private AddFileCommand MapRequestToCommand(AddFileRequest request)
        {
            if (request == null)
                return null;

            return new AddFileCommand()
            {
                Description = request.Description,
                FileName = request.FileName,
            };
        }
    }
}