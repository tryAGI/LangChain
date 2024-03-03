namespace LangChain.Providers.Amazon.SageMaker;

/// <summary>
/// 
/// </summary>
public class SageMakerProvider(
    string apiGatewayRoute)
    : Provider(id: "SageMaker")
{
    #region Properties
    
    public HttpClient HttpClient { get; } = new();
    public Uri Uri { get; } = new(apiGatewayRoute ?? throw new ArgumentNullException(nameof(apiGatewayRoute), "API Gateway Endpoint is not defined"));

    #endregion
}