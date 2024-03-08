namespace LangChain.Databases.OpenSearch;

public partial struct Refresh : IStringable
{
    public static Refresh WaitFor = new("wait_for");
    public static Refresh True = new("true");
    public static Refresh False = new("false");

    public Refresh(string value) => Value = value;

    public string Value { get; }

    public string GetString() => Value ?? string.Empty;
}