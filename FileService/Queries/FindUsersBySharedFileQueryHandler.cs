using System.Collections.Generic;
using System.Linq;
using FileService.Database.EntityFramework;
using FileService.Dto;
using FileService.Model;
using FileService.Services;
using Microsoft.EntityFrameworkCore;

namespace FileService.Queries
{
    public class FindUsersBySharedFileQueryHandler : IQueryHandler<FindUsersBySharedFileQuery, IEnumerable<UserDto>>
    {
        private readonly FileDbContext fileDb;
        private readonly IMapper<User, UserDto> userMapper;

        public FindUsersBySharedFileQueryHandler(FileDbContext fileDb, IMapper<User, UserDto> userMapper)
        {
            this.fileDb = fileDb;
            this.userMapper = userMapper;
        }

        public IEnumerable<UserDto> Handle(FindUsersBySharedFileQuery query)
        {
            return fileDb.Files
                .Where(file => file.Id == query.FileId)
                .Include(f => f.SharedWith)
                .ThenInclude(sharedWith => sharedWith.User)
                .First()
                .SharedWith
                .Select(sh => userMapper.Map(sh.User))
                .ToList();
        }
    }
}