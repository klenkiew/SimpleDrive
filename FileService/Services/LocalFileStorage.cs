using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FileService.Model;
using File = System.IO.File;

namespace FileService.Services
{
    class LocalFileStorage : IFileStorage
    {
        private readonly string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "Content");

        public Task SaveFile(User owner, string fileName, Stream content)
        {
            var filePath = Path.Combine(rootPath, EscapeName(owner.UserName), EscapeName(fileName));
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllBytesAsync(filePath, ReadFully(content)).Wait();
            return Task.CompletedTask;
        }

        public Task<Stream> ReadFile(User owner, string fileName)
        {
            var filePath = Path.Combine(rootPath, EscapeName(owner.UserName), EscapeName(fileName));
            Stream memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
                stream.CopyToAsync(memory).Wait();
            memory.Position = 0;
            return Task.FromResult(memory);
        }

        public void DeleteFile(User owner, string fileName)
        {
            var filePath = Path.Combine(rootPath, EscapeName(owner.UserName), EscapeName(fileName));
            File.Delete(filePath);
        }

        private static byte[] ReadFully(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private string EscapeName(string name)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(name))
                .Replace("+", "_").Replace("/", "-").Replace("=", "");
        }
    }
}