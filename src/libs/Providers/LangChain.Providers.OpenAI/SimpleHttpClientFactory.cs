namespace LangChain.Providers;

internal class SimpleHttpClientFactory : IHttpClientFactory
{
    public HttpClient CreateClient(string name)
    {
        return new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(5),
        };
    }
}