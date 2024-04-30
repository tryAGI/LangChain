using Microsoft.Data.Sqlite;
using LangChain.Serve.Abstractions;
using LangChain.Serve.Abstractions.Repository;

namespace LangChain.Databases.Sqlite;

public sealed class SqLiteConversationRepository : IConversationRepository, IDisposable
{
    private readonly SqliteConnection _connection;

    public SqLiteConversationRepository(string connectionString)
    {
        _connection = new SqliteConnection(connectionString);
        _connection.Open();
        EnsureTables();
    }

    public async Task<StoredConversation> CreateConversation(string modelName)
    {
        var conversation = new StoredConversation
        {
            ConversationId = Guid.NewGuid(),
            ModelName = modelName,
            ConversationName = null,
            CreatedAt = DateTime.UtcNow
        };

        var insertCommand = _connection.CreateCommand();
        insertCommand.CommandText = "INSERT INTO Conversations (ConversationId, ModelName, ConversationName, CreatedAt) VALUES (@ConversationId, @ModelName, @ConversationName, @CreatedAt)";
        insertCommand.Parameters.AddWithValue("@ConversationId", conversation.ConversationId.ToString());
        insertCommand.Parameters.AddWithValue("@ModelName", conversation.ModelName);
        insertCommand.Parameters.AddWithValue("@ConversationName", DBNull.Value);
        insertCommand.Parameters.AddWithValue("@CreatedAt", conversation.CreatedAt);
        await insertCommand.ExecuteNonQueryAsync().ConfigureAwait(false);

        return conversation;
    }

    public async Task UpdateConversationName(Guid conversationId, string conversationName)
    {
        var updateCommand = _connection.CreateCommand();
        updateCommand.CommandText = "UPDATE Conversations SET ConversationName = @ConversationName WHERE ConversationId = @ConversationId";
        updateCommand.Parameters.AddWithValue("@ConversationName", conversationName);
        updateCommand.Parameters.AddWithValue("@ConversationId", conversationId.ToString());
        await updateCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public async Task<StoredConversation?> GetConversation(Guid conversationId)
    {
        var selectCommand = _connection.CreateCommand();
        selectCommand.CommandText = "SELECT * FROM Conversations WHERE ConversationId = @ConversationId";
        selectCommand.Parameters.AddWithValue("@ConversationId", conversationId.ToString());

        using var reader = await selectCommand.ExecuteReaderAsync().ConfigureAwait(false);
        if (await reader.ReadAsync().ConfigureAwait(false))
        {
            return new StoredConversation
            {
                ConversationId = Guid.Parse(reader.GetString(0)),
                ModelName = reader.GetString(1),
                ConversationName = await reader.IsDBNullAsync(2).ConfigureAwait(false) ? null : reader.GetString(2),
                CreatedAt = reader.GetDateTime(3)
            };
        }

        return null;
    }

    public async Task DeleteConversation(Guid conversationId)
    {
        var deleteCommand = _connection.CreateCommand();
        deleteCommand.CommandText = "DELETE FROM Conversations WHERE ConversationId = @ConversationId";
        deleteCommand.Parameters.AddWithValue("@ConversationId", conversationId.ToString());
        await deleteCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public async Task<List<StoredConversation>> ListConversations()
    {
        var selectCommand = _connection.CreateCommand();
        selectCommand.CommandText = "SELECT * FROM Conversations";

        var conversations = new List<StoredConversation>();
        using var reader = await selectCommand.ExecuteReaderAsync().ConfigureAwait(false);
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            conversations.Add(new StoredConversation
            {
                ConversationId = Guid.Parse(reader.GetString(0)),
                ModelName = reader.GetString(1),
                ConversationName = await reader.IsDBNullAsync(2).ConfigureAwait(false) ? null : reader.GetString(2),
                CreatedAt = reader.GetDateTime(3)
            });
        }

        return conversations;
    }

    public async Task AddMessage(StoredMessage message)
    {
        message = message ?? throw new ArgumentNullException(nameof(message));

        var insertCommand = _connection.CreateCommand();
        insertCommand.CommandText = "INSERT INTO Messages (MessageId, ConversationId, Text, Author) VALUES (@MessageId, @ConversationId, @Text, @Author)";
        insertCommand.Parameters.AddWithValue("@MessageId", message.MessageId.ToString());
        insertCommand.Parameters.AddWithValue("@ConversationId", message.ConversationId.ToString());
        insertCommand.Parameters.AddWithValue("@Text", message.Content);
        insertCommand.Parameters.AddWithValue("@Author", message.Author.ToString());
        await insertCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public async Task<List<StoredMessage>> ListMessages(Guid conversationId)
    {
        var selectCommand = _connection.CreateCommand();
        selectCommand.CommandText = "SELECT * FROM Messages WHERE ConversationId = @ConversationId";
        selectCommand.Parameters.AddWithValue("@ConversationId", conversationId.ToString());

        var messages = new List<StoredMessage>();
        using var reader = await selectCommand.ExecuteReaderAsync().ConfigureAwait(false);
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            messages.Add(new StoredMessage
            {
                MessageId = Guid.Parse(reader.GetString(0)),
                ConversationId = Guid.Parse(reader.GetString(1)),
                Content = reader.GetString(2),
                Author = (MessageAuthor)Enum.Parse(typeof(MessageAuthor), reader.GetString(3))
            });
        }

        return messages;
    }

    private void EnsureTables()
    {
        var createConversationsTableCommand = _connection.CreateCommand();
        createConversationsTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS Conversations (ConversationId TEXT PRIMARY KEY, ModelName TEXT, ConversationName TEXT NULL, CreatedAt DATETIME)";
        createConversationsTableCommand.ExecuteNonQuery();

        var createMessagesTableCommand = _connection.CreateCommand();
        createMessagesTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS Messages (MessageId TEXT PRIMARY KEY, ConversationId TEXT, Text TEXT, Author TEXT)";
        createMessagesTableCommand.ExecuteNonQuery();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
