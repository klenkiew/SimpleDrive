using System.Collections.Generic;
using FileService.Commands;
using FileService.Dto;
using FileService.Queries;
using FileService.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Controllers
{
    [Route("api/[controller]s")]
    public class ShareController : ControllerBase
    {
        private readonly ICommandHandler<ShareFileRequest> shareFileCommandHandler;
        private readonly ICommandHandler<UnshareFileCommand> unshareFileCommandHandler;
        private readonly IQueryHandler<FindUsersBySharedFileQuery, IEnumerable<UserDto>> usersBySharedFileQueryHandler;

        public ShareController(
            ICommandHandler<ShareFileRequest> shareFileCommandHandler, 
            ICommandHandler<UnshareFileCommand> unshareFileCommandHandler, 
            IQueryHandler<FindUsersBySharedFileQuery, IEnumerable<UserDto>> usersBySharedFileQueryHandler)
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
            var command = new UnshareFileCommand(fileId, userId);
            unshareFileCommandHandler.Handle(command);
        }
        
        // GET api/shares/{id}
        [HttpGet("{id}")]
        public IEnumerable<UserDto> Get(string id)
        {
            var query = new FindUsersBySharedFileQuery(id);
             return usersBySharedFileQueryHandler.Handle(query);
        }
    }
}