using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileService.Services;
using File = FileService.Model.File;

namespace FileService.Tests.Fakes
{
    public class FakeFileStorage : IFileStorage
    {
        private readonly IDictionary<File, Stream> fileToContentMap = new Dictionary<File, Stream>(); 
        
        public Task SaveFile(File file, Stream content)
        {
            if (!fileToContentMap.TryAdd(file, content))
                throw new InvalidOperationException("The file already exists.");
            return Task.CompletedTask;
        }

        public Task UpdateFile(File file, Stream content)
        {
            if (!fileToContentMap.TryGetValue(file, out _))
                throw new InvalidOperationException("File to update not found.");
            fileToContentMap[file] = content;
            return Task.CompletedTask;
        }

        public Task<Stream> ReadFile(File file)
        {
            if (!fileToContentMap.TryGetValue(file, out Stream content))
                throw new InvalidOperationException("File to read not found.");
            return Task.FromResult(content);
        }

        public Task DeleteFile(File file)
        {
            if (!fileToContentMap.Remove(file))
                throw new InvalidOperationException("File to remove not found.");                
            return Task.CompletedTask;
        }

        public bool Exists(File file)
        {
            return fileToContentMap.ContainsKey(file);
        }
    }
}