namespace LangChain.Splitters;

[TestFixture]
public class CSharpSplitterTests
{
    [Test]
    public async Task Split()
    {
        var splitter = new CSharpSplitter();
        var members = await splitter.SplitAsync(H.Resources.SocketIoClient_cs.AsString()).ConfigureAwait(false);

        members.Select(static x => x.Name).ToArray().Should().BeEquivalentTo(new[]
        {
            "SocketIoClient",
            "EngineIoClient_MessageReceived",
            "GetOnKey",
            "ConnectToNamespacesAsync",
            "ConnectToNamespaceAsync",
            "ConnectAsync",
            "DisconnectAsync",
            "SendEventAsync",
            "WaitEventOrErrorAsync",
            "Emit",
            "Off",
            "On",
            "Dispose",
        });
    }

    [Test]
    public async Task Cut()
    {
        var content = H.Resources.SocketIoClient_cs.AsString();
        var splitter = new CSharpSplitter();
        var members = await splitter.SplitAsync(content).ConfigureAwait(false);

        var newContent = await splitter.CutAsync(
            content,
            members
                .Select(static x => x.Name)
                .Where(static x =>
                    x != "SocketIoClient" &&
                    x != "EngineIoClient_MessageReceived")
                .ToArray()).ConfigureAwait(false);

        var newMembers = await splitter.SplitAsync(newContent).ConfigureAwait(false);
        newMembers.Select(static x => x.Name).ToArray().Should().BeEquivalentTo(new[]
        {
            "GetOnKey",
            "ConnectToNamespacesAsync",
            "ConnectToNamespaceAsync",
            "ConnectAsync",
            "DisconnectAsync",
            "SendEventAsync",
            "WaitEventOrErrorAsync",
            "Emit",
            "Off",
            "On",
            "Dispose",
        });

        Console.WriteLine("New content:");
        Console.WriteLine(newContent);
    }
}