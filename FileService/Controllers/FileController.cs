using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FileService.Commands;
using FileService.Model;
using FileService.Queries;
using FileService.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using File = FileService.Model.File;

namespace FileService.Controllers
{
    [Route("api/[controller]s")]
    public class FileController : Controller
    {
        private readonly ICommandHandler<AddFileCommand> addFileCommandHandler;
        private readonly ICommandHandler<DeleteFileCommand> deleteFileCommandHandler;
        private readonly IQueryHandler<FindFilesByOwnerQuery, IEnumerable<File>> findFilesQueryHandler;
        private readonly IQueryHandler<GetFileContentQuery, Stream> getFileContentQueryHandler;

        public FileController(
            ICommandHandler<AddFileCommand> addFileCommandHandler,
            ICommandHandler<DeleteFileCommand> deleteFileCommandHandler, 
            IQueryHandler<FindFilesByOwnerQuery, IEnumerable<File>> findFilesQueryHandler, 
            IQueryHandler<GetFileContentQuery, Stream> getFileContentQueryHandler)
        {
            this.addFileCommandHandler =
                addFileCommandHandler ?? throw new ArgumentNullException(nameof(addFileCommandHandler));
            
            this.deleteFileCommandHandler = 
                deleteFileCommandHandler ?? throw new ArgumentNullException(nameof(addFileCommandHandler));;
            
            this.findFilesQueryHandler =
                findFilesQueryHandler ?? throw new ArgumentNullException(nameof(findFilesQueryHandler));
            
            this.getFileContentQueryHandler = 
                getFileContentQueryHandler ?? throw new ArgumentNullException(nameof(getFileContentQueryHandler));
        }

        // GET api/files
        [HttpGet]
        public IEnumerable<File> Get()
        {
            return findFilesQueryHandler.Handle(new FindFilesByOwnerQuery() {OwnerId = GetCurrentUser().Id});
        }

        // GET api/files/5
        [HttpGet("{id}")]
        public string Get(string id)
        {
            throw new NotImplementedException();
        }

        // GET api/files/5/content
        [HttpGet("{id}/content")]
        public IActionResult GetContent(string id)
        {
            var query = new GetFileContentQuery()
            {
                FileId = id,
                Owner = GetCurrentUser()
            };
            var fileContentStream = getFileContentQueryHandler.Handle(query);
            return new FileStreamResult(fileContentStream, "text/plain");
        }

        // POST api/files
        [HttpPost]
        public void Post([FromForm] AddFileRequest addFileRequest)
        {
            var command = new AddFileCommand()
            {
                FileName = addFileRequest.FileName,
                Description = addFileRequest.Description,
                Owner = GetCurrentUser(),
                Content = addFileRequest.File.OpenReadStream(),
            };

            addFileCommandHandler.Handle(command);
        }

        // PUT api/files/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            throw new NotImplementedException();
        }

        // DELETE api/files/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            var command = new DeleteFileCommand()
            {
                FileId = id,
                Owner = GetCurrentUser()
            };

            deleteFileCommandHandler.Handle(command);
        }

        private User GetCurrentUser()
        {
            return new User()
            {
                Id = HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value,
                UserName = HttpContext.User.Identity.Name,
            };
        }
    }
}