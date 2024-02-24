// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable

/// <summary>
/// Base class for embedding request settings.
/// </summary>
public class EmbeddingSettings
{
    public static EmbeddingSettings Default { get; } = new();
}