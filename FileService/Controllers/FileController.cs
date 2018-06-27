using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileService.Commands;
using FileService.Model;
using FileService.Queries;
using FileService.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Controllers
{
    [Authorize]
    [Route("api/[controller]s")]
    public class FileController : Controller
    {
        private readonly ICommandHandler<AddFileCommand> addFileCommandHandler;
        private readonly ICommandHandler<DeleteFileCommand> deleteFileCommandHandler;
        private readonly IQueryHandler<FindFilesByOwnerQuery, IEnumerable<File>> findFilesQueryHandler;

        public FileController(
            ICommandHandler<AddFileCommand> addFileCommandHandler,
            ICommandHandler<DeleteFileCommand> deleteFileCommandHandler, 
            IQueryHandler<FindFilesByOwnerQuery, IEnumerable<File>> findFilesQueryHandler)
        {
            this.addFileCommandHandler =
                addFileCommandHandler ?? throw new ArgumentNullException(nameof(addFileCommandHandler));
            
            this.deleteFileCommandHandler = 
                deleteFileCommandHandler ?? throw new ArgumentNullException(nameof(addFileCommandHandler));;
            
            this.findFilesQueryHandler =
                findFilesQueryHandler ?? throw new ArgumentNullException(nameof(findFilesQueryHandler));
        }

        // GET api/files
        [HttpGet]
        public IEnumerable<File> Get()
        {
            return findFilesQueryHandler.Handle(new FindFilesByOwnerQuery() {OwnerId = HttpContext.User.Identity.Name});
        }

        // GET api/files/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            throw new NotImplementedException();
        }

        // POST api/files
        [HttpPost]
        public void Post([FromForm] AddFileRequest addFileRequest)
        {
            var command = new AddFileCommand()
            {
                FileName = addFileRequest.FileName,
                Description = addFileRequest.Description,
                OwnerUserId = HttpContext.User.Identity.Name,
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
                OwnerUserId = HttpContext.User.Identity.Name
            };

            deleteFileCommandHandler.Handle(command);
        }
    }
}