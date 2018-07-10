using System.Threading.Tasks;
using Serialization;

namespace FileService.Infrastructure.HttpClient
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly IHttpClientAccessor httpClientAccessor;
        private readonly ISerializer serializer;

        private System.Net.Http.HttpClient Client => httpClientAccessor.Client;
        
        public HttpClientWrapper(IHttpClientAccessor httpClientAccessor, ISerializer serializer)
        {
            this.httpClientAccessor = httpClientAccessor;
            this.serializer = serializer;
        }

        public T Get<T>(string url)
        {
            var response = Client.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();;
            return serializer.Deserialize<T>(content);
        }

        public async Task<T> GetAsync<T>(string url)
        {
            var response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return serializer.Deserialize<T>(content);
        }
    }
}