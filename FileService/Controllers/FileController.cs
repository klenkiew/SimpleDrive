using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using FileService.Commands;
using FileService.Model;
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
        private readonly IQueryHandler<FindFilesByUserQuery, IEnumerable<File>> findFilesQueryHandler;
        private readonly IQueryHandler<FindFileByIdQuery, File> findFileByIdQueryHandler;
        private readonly IQueryHandler<GetFileContentQuery, Stream> getFileContentQueryHandler;

        public FileController(
            ICurrentUser currentUser,
            ICommandHandler<AddFileRequest> addFileCommandHandler,
            ICommandHandler<DeleteFileCommand> deleteFileCommandHandler, 
            IQueryHandler<FindFilesByUserQuery, IEnumerable<File>> findFilesQueryHandler, 
            IQueryHandler<FindFileByIdQuery, File> findFileByIdQueryHandler, 
            IQueryHandler<GetFileContentQuery, Stream> getFileContentQueryHandler)
        {
            this.currentUser =
                currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            ;
            this.addFileCommandHandler =
                addFileCommandHandler ?? throw new ArgumentNullException(nameof(addFileCommandHandler));
            
            this.deleteFileCommandHandler = 
                deleteFileCommandHandler ?? throw new ArgumentNullException(nameof(addFileCommandHandler));;
            
            this.findFilesQueryHandler =
                findFilesQueryHandler ?? throw new ArgumentNullException(nameof(findFilesQueryHandler));

            this.findFileByIdQueryHandler = 
                findFileByIdQueryHandler ?? throw new ArgumentNullException(nameof(findFileByIdQueryHandler));
            
            this.getFileContentQueryHandler = 
                getFileContentQueryHandler ?? throw new ArgumentNullException(nameof(getFileContentQueryHandler));
        }

        // GET api/files
        [HttpGet]
        public IEnumerable<File> Get()
        {
            return findFilesQueryHandler.Handle(new FindFilesByUserQuery() {UserId = currentUser.Id});
        }

        // GET api/files/5
        [HttpGet("{id}")]
        public File Get(string id)
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
            var fileContentStream = getFileContentQueryHandler.Handle(query);
            return new FileStreamResult(fileContentStream, "text/plain");
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