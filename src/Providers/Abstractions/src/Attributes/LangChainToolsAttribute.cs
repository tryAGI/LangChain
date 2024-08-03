// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Interface)]
[System.Diagnostics.Conditional("LANGCHAIN_TOOLS_ATTRIBUTES")]
public sealed class LangChainToolsAttribute : Attribute;