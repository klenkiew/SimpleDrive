using System;
using System.Linq;
using FileService.Database;
using FileService.Exceptions;
using FileService.Model;
using FileService.Services;
using Microsoft.EntityFrameworkCore;

namespace FileService.Commands
{
    public class UpdateFileContentCommandHandler : ICommandHandler<UpdateFileContentCommand>
    {
        private readonly IFileStorage fileStorage;
        private readonly IFileRepository fileRepository;
        private readonly ICurrentUser currentUser;

        public UpdateFileContentCommandHandler(
            IFileStorage fileStorage, 
            IFileRepository fileRepository, 
            ICurrentUser currentUser)
        {
            this.fileStorage = fileStorage;
            this.fileRepository = fileRepository;
            this.currentUser = currentUser;
        }

        public void Handle(UpdateFileContentCommand command)
        {
            File file = fileRepository.GetById(command.FileId).EnsureFound(command.FileId);
            
            if (!file.CanBeModifiedBy(currentUser.ToDomainUser()))
                throw new PermissionException($"The user doesn't have a permission to update the content of the file with id {command.FileId}");

            file.ContentChanged(DateTime.Now);
            fileStorage.UpdateFile(file, command.Content);
        }
    }
}