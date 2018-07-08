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
        private readonly ICommandHandler<ShareFileCommand> shareFileCommandHandler;
        private readonly ICommandHandler<UnshareFileCommand> unshareFileCommandHandler;
        private readonly IQueryHandler<FindUsersBySharedFileQuery, IEnumerable<User>> usersBySharedFileQueryHandler;

        public ShareController(
            ICommandHandler<ShareFileCommand> shareFileCommandHandler, 
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
            var command = new ShareFileCommand()
            {
                FileId =  shareFileRequest.FileId,
                OwnerId = HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value,
                SharedWithUserId = shareFileRequest.ShareWithUserId
            };

            shareFileCommandHandler.Handle(command);
        }
        
        // DELETE api/shares
        [HttpDelete]
        public void Delete([FromQuery] string fileId, [FromQuery] string userId)
        {
            var command = new UnshareFileCommand()
            {
                FileId =  fileId,
                OwnerId = HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value,
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