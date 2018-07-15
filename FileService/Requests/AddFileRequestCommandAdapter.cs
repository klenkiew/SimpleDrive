using FileService.Commands;
using FileService.Services;

namespace FileService.Requests
{
    public class AddFileRequestCommandAdapter : ICommandHandler<AddFileRequest>
    {
        private readonly ICommandHandler<AddFileCommand> adaptee;
        private readonly IMimeTypeMap mimeTypeMap;

        public AddFileRequestCommandAdapter(ICommandHandler<AddFileCommand> adaptee, IMimeTypeMap mimeTypeMap)
        {
            this.adaptee = adaptee;
            this.mimeTypeMap = mimeTypeMap;
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

            var contentType = mimeTypeMap.GetMimeType(request.File.FileName) 
                              ?? mimeTypeMap.GetMimeType(request.FileName);

            return new AddFileCommand()
            {
                Description = request.Description,
                FileName = request.FileName,
                MimeType = contentType
            };
        }
    }
}