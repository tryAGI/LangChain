using GenerativeAI.Models;

namespace LangChain.Providers.Models;

/// <summary>
/// Google Gemini Pro Model
/// </summary>
public class GeminiProModel : GenerativeModel
{
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="httpClient"></param>
    public GeminiProModel(string apiKey, HttpClient httpClient) : base(apiKey, httpClient, GoogleAIModels.GeminiPro)
    {
            
    }
}