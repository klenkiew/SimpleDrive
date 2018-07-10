namespace FileService.Infrastructure.HttpClient
{
    public class HttpClientAccessor : IHttpClientAccessor
    {
        private static readonly System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();

        public System.Net.Http.HttpClient Client => httpClient;
    }
}