namespace LangChain.Abstractions.Schema;

public interface IInputValues
{
    Dictionary<string, object> Value { get; }
}