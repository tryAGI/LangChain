#if NET7_0_OR_GREATER
namespace LangChain.Sources;

#pragma warning disable CA1000
#pragma warning disable CA1711

public interface ICreatableFromStream<out TSelf> where TSelf : ICreatableFromStream<TSelf>
{
    public static abstract TSelf CreateFromStream(Stream stream);
}
#endif