using System.Collections.Generic;
using System.Linq;
using FileService.Database;
using FileService.Database.EntityFramework;
using FileService.Dto;
using FileService.Model;
using Microsoft.EntityFrameworkCore;

namespace FileService.Queries
{
    public class FindUsersBySharedFileQueryHandler : IQueryHandler<FindUsersBySharedFileQuery, IEnumerable<UserDto>>
    {
        private readonly FileDbContext fileDb;

        public FindUsersBySharedFileQueryHandler(FileDbContext fileDb)
        {
            this.fileDb = fileDb;
        }

        public IEnumerable<UserDto> Handle(FindUsersBySharedFileQuery query)
        {
            return fileDb.Files
                .Where(file => file.Id == query.FileId)
                .Include(f => f.SharedWith)
                .ThenInclude(sharedWith => sharedWith.User)
                .First()
                .SharedWith
                .Select(sh => new UserDto(sh.User.Id, sh.User.Username, "N/A"))
                .ToList();
        }
    }
}