// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class SpeechToTextResponse
{
    public required string Text { get; init; }
    
    public Usage Usage { get; init; } = Usage.Empty;
    
    public void Deconstruct(
        out string values,
        out Usage usage)
    {
        values = Text;
        usage = Usage;
    }
    
    public static implicit operator string(SpeechToTextResponse response)
    {
        return response?.ToString() ?? string.Empty;
    }

    public override string ToString()
    {
        return Text;
    }
}