using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FileService.Commands;
using FileService.Model;
using FileService.Queries;
using FileService.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Controllers
{
    [Route("api/[controller]s")]
    public class ShareController : Controller
    {
        private readonly ICommandHandler<ShareFileRequest> shareFileCommandHandler;
        private readonly ICommandHandler<UnshareFileCommand> unshareFileCommandHandler;
        private readonly IQueryHandler<FindUsersBySharedFileQuery, IEnumerable<User>> usersBySharedFileQueryHandler;

        public ShareController(
            ICommandHandler<ShareFileRequest> shareFileCommandHandler, 
            ICommandHandler<UnshareFileCommand> unshareFileCommandHandler, 
            IQueryHandler<FindUsersBySharedFileQuery, IEnumerable<User>> usersBySharedFileQueryHandler)
        {
            this.shareFileCommandHandler = shareFileCommandHandler;
            this.unshareFileCommandHandler = unshareFileCommandHandler;
            this.usersBySharedFileQueryHandler = usersBySharedFileQueryHandler;
        }

        // POST api/shares
        [HttpPost]
        public void Post([FromBody] ShareFileRequest shareFileRequest)
        {
            shareFileCommandHandler.Handle(shareFileRequest);
        }
        
        // DELETE api/shares
        [HttpDelete]
        public void Delete([FromQuery] string fileId, [FromQuery] string userId)
        {
            var command = new UnshareFileCommand()
            {
                FileId =  fileId,
                UserId = userId
            };

            unshareFileCommandHandler.Handle(command);
        }
        
        // GET api/shares/{id}
        [HttpGet("{id}")]
        public IEnumerable<User> Get(string id)
        {
            var query = new FindUsersBySharedFileQuery()
            {
                FileId = id
            };

            return usersBySharedFileQueryHandler.Handle(query);
        }
    }
}