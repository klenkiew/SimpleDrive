using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FileService.Commands;
using FileService.Dto;
using FileService.Queries;
using FileService.Requests;
using FileService.Services;
using Microsoft.AspNetCore.Mvc;
using File = FileService.Model.File;

namespace FileService.Controllers
{
    [Route("api/[controller]s")]
    public class FileController : Controller
    {
        private readonly ICurrentUser currentUser;
        private readonly ICommandHandler<AddFileRequest> addFileCommandHandler;
        private readonly ICommandHandler<DeleteFileCommand> deleteFileCommandHandler;
        private readonly ICommandHandler<UpdateFileContentCommand> updateFileContentCommandHandler;
        private readonly IQueryHandler<FindFilesByUserQuery, IEnumerable<FileDto>> findFilesQueryHandler;
        private readonly IQueryHandler<FindFileByIdQuery, FileDto> findFileByIdQueryHandler;
        private readonly IQueryHandler<GetFileContentQuery, FileContentDto> getFileContentQueryHandler;

        public FileController(
            ICurrentUser currentUser,
            ICommandHandler<AddFileRequest> addFileCommandHandler,
            ICommandHandler<DeleteFileCommand> deleteFileCommandHandler, 
            ICommandHandler<UpdateFileContentCommand> updateFileContentCommandHandler, 
            IQueryHandler<FindFilesByUserQuery, IEnumerable<FileDto>> findFilesQueryHandler, 
            IQueryHandler<FindFileByIdQuery, FileDto> findFileByIdQueryHandler, 
            IQueryHandler<GetFileContentQuery, FileContentDto> getFileContentQueryHandler)
        {
            this.currentUser =
                currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            ;
            this.addFileCommandHandler =
                addFileCommandHandler ?? throw new ArgumentNullException(nameof(addFileCommandHandler));
            
            this.deleteFileCommandHandler = 
                deleteFileCommandHandler ?? throw new ArgumentNullException(nameof(addFileCommandHandler));;

            this.updateFileContentCommandHandler = 
                updateFileContentCommandHandler ?? throw new ArgumentNullException(nameof(updateFileContentCommandHandler));
            
            this.findFilesQueryHandler =
                findFilesQueryHandler ?? throw new ArgumentNullException(nameof(findFilesQueryHandler));

            this.findFileByIdQueryHandler = 
                findFileByIdQueryHandler ?? throw new ArgumentNullException(nameof(findFileByIdQueryHandler));

            this.getFileContentQueryHandler = 
                getFileContentQueryHandler ?? throw new ArgumentNullException(nameof(getFileContentQueryHandler));
        }

        // GET api/files
        [HttpGet]
        public IEnumerable<FileDto> Get()
        {
            return findFilesQueryHandler.Handle(new FindFilesByUserQuery() {UserId = currentUser.Id});
        }

        // GET api/files/5
        [HttpGet("{id}")]
        public FileDto Get(string id)
        {
            return findFileByIdQueryHandler.Handle(new FindFileByIdQuery() {FileId = id});
        }

        // GET api/files/5/content
        [HttpGet("{id}/content")]
        public IActionResult GetContent(string id)
        {
            var query = new GetFileContentQuery()
            {
                FileId = id,
            };
            var fileContentInfo = getFileContentQueryHandler.Handle(query);
            return new FileStreamResult(fileContentInfo.Content, fileContentInfo.MimeType ?? "application/octet-stream");
        }

        // PUT api/files/5/content
        [HttpPut("{id}/content")]
        public void UpdateContent([FromBody] UpdateContentRequest request)
        {
            // TODO content as Stream in the request instead of string
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(request.Content)))
            {
                var command = new UpdateFileContentCommand()
                {
                    FileId = request.FileId,
                    Content = memoryStream
                };
                updateFileContentCommandHandler.Handle(command);
            }
        }
        
        // POST api/files
        [HttpPost]
        public void Post([FromForm] AddFileRequest addFileRequest)
        {
            addFileCommandHandler.Handle(addFileRequest);
        }
        
        // DELETE api/files/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            var command = new DeleteFileCommand()
            {
                FileId = id,
            };

            deleteFileCommandHandler.Handle(command);
        }
    }
}