using System;
using System.IO;
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
            using (var content = request.File?.OpenReadStream())
            {
                var command = MapRequestToCommand(request, content);
                adaptee.Handle(command);
            }
        }

        private AddFileCommand MapRequestToCommand(AddFileRequest request, Stream content)
        {
            if (request == null)
                return null;

            var contentType = mimeTypeMap.GetMimeType(request.File.FileName) 
                              ?? mimeTypeMap.GetMimeType(request.FileName);

            return new AddFileCommand(request.FileName, request.Description, contentType, content);
        }
    }
}