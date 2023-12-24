using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenerativeAI.Models;

namespace LangChain.Providers.Models
{
    /// <summary>
    /// Google Gemini Pro Model
    /// </summary>
    public class GeminiProModel : GenerativeModel
    {
        public GeminiProModel(string apiKey, HttpClient httpClient) : base(apiKey, httpClient, GoogleAIModels.GeminiPro)
        {
            
        }
    }
}
