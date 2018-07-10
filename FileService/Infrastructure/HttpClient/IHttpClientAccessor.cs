namespace FileService.Infrastructure.HttpClient
{
    public interface IHttpClientAccessor
    {
        System.Net.Http.HttpClient Client { get; }
    }
}