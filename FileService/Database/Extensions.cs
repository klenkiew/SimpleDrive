using FileService.Exceptions;
using FileService.Model;

namespace FileService.Database
{
    public static class FileExtensions
    {
        public static File EnsureFound(this File file, string id)
        {
            return file ?? throw new NotFoundException($"A file with id {id} doesn't exist in the database.");
        }
    }
}