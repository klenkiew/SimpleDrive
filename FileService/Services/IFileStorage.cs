using System.IO;
using System.Threading.Tasks;
using File = FileService.Model.File;

namespace FileService.Services
{
    public interface IFileStorage
    {
        Task SaveFile(File file, Stream content);
        Task UpdateFile(File file, Stream content);
        Task<Stream> ReadFile(File file);
        Task DeleteFile(File file);
    }
}