using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileService.Configuration;
using FileService.Exceptions;
using FileModel = FileService.Model.File;

namespace FileService.Services
{
    internal class LocalFileStorage : IFileStorage
    {
        private readonly string rootPath;

        public LocalFileStorage(StorageConfiguration configuration)
        {
            rootPath = configuration.Path;
            // ensure that the path is valid and indicates an accessible directory to prevent issues during requests 
            Directory.CreateDirectory(rootPath);
        }

        public async Task SaveFile(FileModel file, Stream content)
        {
            var filePath = GetFilePath(file);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                await content.CopyToAsync(fileStream);
        }

        public async Task UpdateFile(FileModel file, Stream content)
        {
            var filePath = GetFilePath(file);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Write))
                await content.CopyToAsync(fileStream);
        }

        public Task<Stream> ReadFile(FileModel file)
        {
            try
            {
                var filePath = GetFilePath(file);
                return ReadFile(filePath);
            }
            catch (FileNotFoundException ex)
            {
                throw new NotFoundException($"A file with id {file.Id} was not found in the storage.", ex);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException)
            {
                throw new StorageAccessException("An error occured during accessing the file storage", ex);
            }
        }

        public void DeleteFile(FileModel file)
        {
            var filePath = Path.Combine(rootPath, EscapeName(file.OwnerName), EscapeName(file.FileName));
            File.Delete(filePath);
            
            // clear the parent directory if it's empty after deleting the file
            DeleteParentDirectoryIfEmpty(filePath);
        }

        private Task<Stream> ReadFile(string filePath)
        {
            Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return Task.FromResult(stream);
        }

        private static void DeleteParentDirectoryIfEmpty(string filePath)
        {
            var parentDirectory = Directory.GetParent(filePath);
            if (!parentDirectory.GetFiles().Any())
                Directory.Delete(parentDirectory.FullName);
        }

        private string GetFilePath(FileModel file)
        {
            return Path.Combine(rootPath, EscapeName(file.OwnerName), EscapeName(file.FileName));
        }

        private string EscapeName(string name)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(name))
                .Replace("+", "_").Replace("/", "-").Replace("=", "");
        }
    }
}