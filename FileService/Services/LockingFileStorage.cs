using System.IO;
using System.Threading.Tasks;
using File = FileService.Model.File;

namespace FileService.Services
{
    class LockingFileStorage : IFileStorage
    {
        private readonly IFileStorage decorated;
        private readonly IFileLockingService fileLockingService;

        public LockingFileStorage(IFileStorage decorated, IFileLockingService fileLockingService)
        {
            this.decorated = decorated;
            this.fileLockingService = fileLockingService;
        }

        public async Task SaveFile(File file, Stream content)
        {
            await decorated.SaveFile(file, content);
        }

        public async Task UpdateFile(File file, Stream content)
        {
            using (fileLockingService.CreateLock(file))
                await decorated.UpdateFile(file, content);
        }

        public async Task<Stream> ReadFile(File file)
        {
            return await decorated.ReadFile(file);
        }

        public async Task DeleteFile(File file)
        {
            using (fileLockingService.CreateLock(file))
                await decorated.DeleteFile(file);
        }
    }
}