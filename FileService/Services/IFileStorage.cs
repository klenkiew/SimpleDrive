using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileService.Model;
using Microsoft.Extensions.FileProviders;

namespace FileService.Services
{
    public interface IFileStorage
    {
        Task SaveFile(User owner, string fileName, Stream content);
        Task<Stream> ReadFile(User owner, string fileName);
        void DeleteFile(User owner, string fileName);
    }
}