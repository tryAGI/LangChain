namespace LangChain.Providers.Amazon.SageMaker;

/// <summary>
/// 
/// </summary>
public class SageMakerProvider(
    string apiGatewayEndpoint)
    : Provider(id: "SageMaker")
{
    #region Properties
    
    public HttpClient HttpClient { get; } = new();
    public Uri Uri { get; } = new(apiGatewayEndpoint ?? throw new ArgumentNullException(nameof(apiGatewayEndpoint), "API Gateway Endpoint is not defined"));
    
    #endregion
}