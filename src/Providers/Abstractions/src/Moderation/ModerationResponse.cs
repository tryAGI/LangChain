// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class ModerationResponse
{
    public required bool IsValid { get; init; }

    public Usage Usage { get; init; } = Usage.Empty;

    public void Deconstruct(
        out bool isValid,
        out Usage usage)
    {
        isValid = IsValid;
        usage = Usage;
    }

    public static implicit operator bool(ModerationResponse response)
    {
        return response?.ToBoolean() ?? false;
    }

    public bool ToBoolean()
    {
        return IsValid;
    }
}