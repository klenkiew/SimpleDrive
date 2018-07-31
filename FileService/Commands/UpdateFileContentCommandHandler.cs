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
        private readonly IRepository<File> fileRepository;
        private readonly ICurrentUser currentUser;
        private readonly IPostCommitRegistrator registrator;

        public UpdateFileContentCommandHandler(
            IFileStorage fileStorage, 
            IRepository<File> fileRepository, 
            ICurrentUser currentUser, 
            IPostCommitRegistrator registrator)
        {
            this.fileStorage = fileStorage;
            this.fileRepository = fileRepository;
            this.currentUser = currentUser;
            this.registrator = registrator;
        }

        public void Handle(UpdateFileContentCommand command)
        {
            File file = fileRepository.GetById(command.FileId).EnsureFound(command.FileId);
            
            if (!file.CanBeModifiedBy(currentUser.ToDomainUser()))
                throw new PermissionException($"The user doesn't have a permission to update the content of the file with id {command.FileId}");

            file.ContentChanged(DateTime.Now);
            fileStorage.UpdateFile(file, command.Content);
//            dbContext.SaveChanges();
        }
    }
}