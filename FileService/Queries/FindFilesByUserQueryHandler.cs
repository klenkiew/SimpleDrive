using System.Collections.Generic;
using System.Linq;
using FileService.Database.EntityFramework;
using FileService.Dto;
using FileService.Model;
using FileService.Services;

namespace FileService.Queries
{
    public class FindFilesByUserQueryHandler : IQueryHandler<FindFilesByUserQuery, IEnumerable<FileDto>>
    {
        private readonly FileDbContext fileDb;
        private readonly IMapper<File, FileDto> fileMapper;
        
        public FindFilesByUserQueryHandler(FileDbContext fileDb, IMapper<File, FileDto> fileMapper)
        {
            this.fileDb = fileDb;
            this.fileMapper = fileMapper;
        }

        public IEnumerable<FileDto> Handle(FindFilesByUserQuery query)
        {
            return fileDb.Files
                .Where(file => file.Owner.Id == query.UserId || file.SharedWith.Any(sw => sw.UserId == query.UserId))
                .Select(file => fileMapper.Map(file))
                .ToList();
        }
    }
}