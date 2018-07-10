using System.Threading.Tasks;

namespace FileService.Infrastructure.HttpClient
{
    public interface IHttpClientWrapper
    {
        T Get<T>(string url);
        Task<T> GetAsync<T>(string url);
    }
}