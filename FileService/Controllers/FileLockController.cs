﻿using System;
using FileService.Commands;
using FileService.Dto;
using FileService.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Controllers
{
    [Route("api/files/{id}/lock")]
    public class FileLockController : ControllerBase
    {
        private readonly ICommandHandler<AcquireFileLockCommand> acquireFileLockCommandHandler;
        private readonly ICommandHandler<RemoveFileLockCommand> removeFileLockCommandHandler;
        private readonly IQueryHandler<GetFileLockQuery, FileLockDto> getFileLockQueryHandler;

        public FileLockController(
            ICommandHandler<AcquireFileLockCommand> acquireFileLockCommandHandler, 
            ICommandHandler<RemoveFileLockCommand> removeFileLockCommandHandler, 
            IQueryHandler<GetFileLockQuery, FileLockDto> getFileLockQueryHandler)
        {
            this.removeFileLockCommandHandler =
                removeFileLockCommandHandler ?? throw new ArgumentNullException(nameof(removeFileLockCommandHandler));
            
            this.acquireFileLockCommandHandler = 
                acquireFileLockCommandHandler ?? throw new ArgumentNullException(nameof(acquireFileLockCommandHandler));
            
            this.getFileLockQueryHandler = 
                getFileLockQueryHandler ?? throw new ArgumentNullException(nameof(getFileLockQueryHandler));
        }

        // GET api/files/5/lock
        [HttpGet]
        public FileLockDto GetLock(string id)
        {
            var query = new GetFileLockQuery(id);
            return getFileLockQueryHandler.Handle(query);
        }
        
        // POST api/files/5/lock
        [HttpPost]
        public void PostLock(string id)
        {
            var command = new AcquireFileLockCommand(id);
            acquireFileLockCommandHandler.Handle(command);
        }
        
        // DELETE api/files/5/lock
        [HttpDelete]
        public void DeleteLock(string id)
        {
            var command = new RemoveFileLockCommand(id);
            removeFileLockCommandHandler.Handle(command);
        }
    }
}