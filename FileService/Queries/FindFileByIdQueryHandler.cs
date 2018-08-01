using System.Linq;
using FileService.Database.EntityFramework;
using FileService.Dto;
using FileService.Model;
using FileService.Services;
using Microsoft.EntityFrameworkCore;

namespace FileService.Queries
{
    public class FindFileByIdQueryHandler : IQueryHandler<FindFileByIdQuery, FileDto>
    {
        private readonly FileDbContext fileDb;
        private readonly IMapper<File, FileDto> fileMapper;
        
        public FindFileByIdQueryHandler(FileDbContext fileDb, IMapper<File, FileDto> fileMapper)
        {
            this.fileDb = fileDb;
            this.fileMapper = fileMapper;
        }

        public FileDto Handle(FindFileByIdQuery query)
        {
            File file = fileDb.Files
                .Include(f => f.Owner)
                .FirstOrDefault(f => f.Id == query.FileId)
                .EnsureFound(query.FileId);
            
            return fileMapper.Map(file);
        }
    }
}