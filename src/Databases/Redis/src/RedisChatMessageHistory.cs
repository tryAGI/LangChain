using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using LangChain.Memory;
using LangChain.Providers;
using StackExchange.Redis;

namespace LangChain.Databases;

/// <summary>
/// Chat message history stored in a Redis database.
/// </summary>
[RequiresDynamicCode("Requires dynamic code.")]
[RequiresUnreferencedCode("Requires unreferenced code.")]
public class RedisChatMessageHistory : BaseChatMessageHistory
{
    public TimeSpan? Ttl { get; set; }

    private readonly string _sessionId;
    private readonly string _keyPrefix;
    private readonly Lazy<ConnectionMultiplexer> _multiplexer;

    /// <inheritdoc />
    public RedisChatMessageHistory(
        string sessionId,
        string connectionString,
        string keyPrefix = "message_store:",
        TimeSpan? ttl = null)
    {
        _sessionId = sessionId;
        _keyPrefix = keyPrefix;
        Ttl = ttl;

        _multiplexer = new Lazy<ConnectionMultiplexer>(
            () =>
            {
                var multiplexer = ConnectionMultiplexer.Connect(connectionString);

                return multiplexer;
            },
            LazyThreadSafetyMode.ExecutionAndPublication);
    }

    /// <summary>
    /// Construct the record key to use
    /// </summary>
    private string Key => _keyPrefix + _sessionId;

    /// <summary>
    /// Retrieve the messages from Redis
    /// TODO: use async methods
    /// </summary>
    public override IReadOnlyList<Message> Messages
    {
        get
        {
            var database = _multiplexer.Value.GetDatabase();
            var values = database.ListRange(Key, start: 0, stop: -1);
            var messages = values.Select(v => JsonSerializer.Deserialize<Message>(v.ToString())).Reverse();

            return messages.ToList();
        }
    }

    /// <summary>
    /// Append the message to the record in Redis
    /// </summary>
    public override async Task AddMessage(Message message)
    {
        var database = _multiplexer.Value.GetDatabase();
        await database.ListLeftPushAsync(Key, JsonSerializer.Serialize(message)).ConfigureAwait(false);
        if (Ttl.HasValue)
        {
            await database.KeyExpireAsync(Key, Ttl).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Clear session memory from Redis
    /// </summary>
    public override async Task Clear()
    {
        var database = _multiplexer.Value.GetDatabase();
        await database.KeyDeleteAsync(Key).ConfigureAwait(false);
    }
}