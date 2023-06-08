namespace LangChain.Providers;

internal sealed class SimpleHttpClientFactory : IHttpClientFactory
{
    public HttpClient CreateClient(string name)
    {
        return new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(5),
        };
    }
}