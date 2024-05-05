// ReSharper disable once CheckNamespace
namespace Reka;

/// <summary>
/// 
/// </summary>
public partial class RekaApi
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="httpClient"></param>
    /// <param name="baseUrl"></param>
    public RekaApi(string token, HttpClient httpClient, string? baseUrl = null) : this(baseUrl ?? "https://api.reka.ai/", httpClient)
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    }
}
