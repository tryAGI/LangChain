using LangChain.Chains.LLM;
using LangChain.Providers.OpenAI;
using LangChain.Utilities.SqlDatabases;
using Npgsql;
using OpenAI.Constants;

namespace LangChain.Utilities.PostgresDatabases.IntegrationTests;

/// <summary>
/// In order to run tests please run postgres locally, e.g. with docker
/// docker run -p 5432:5432 postgres -e POSTGRES_PASSWORD=password -e POSTGRES_DB=test
/// </summary>
[TestFixture]
[Explicit]
public class PostgresTests
{
    private readonly string _ownerConnectionString;
    private readonly string _readonlyConnectionString;

    public PostgresTests()
    {
        const string host = "localhost";
        const int port = 5432;

        _ownerConnectionString = $"Host={host};Port={port};Database=test;User ID=postgres;Password=password;";
        _readonlyConnectionString = $"Host={host};Port={port};Database=test;User ID=readonly;Password=password;";
    }

    [OneTimeSetUp]
    public void Init()
    {
        var initDbCommandText = @"
DROP ROLE IF EXISTS readonly;
DROP TABLE IF EXISTS pets;
DROP TABLE IF EXISTS kids;
DROP TYPE IF EXISTS pet_type;

CREATE ROLE readonly WITH LOGIN PASSWORD 'password';
GRANT SELECT ON ALL TABLES IN SCHEMA public TO readonly;


CREATE TABLE kids
(
    id BIGSERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    age INT CHECK (age > 0)
);

INSERT INTO kids(name, age)
VALUES
    ('jack', 10),
    ('nancy', 5),
    ('jimmy', 7);

CREATE TYPE pet_type AS ENUM ('cat', 'dog');

CREATE TABLE pets
(
    id BIGSERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    type pet_type NOT NULL,
    owner_id BIGINT REFERENCES kids(id) ON DELETE CASCADE
);

INSERT INTO pets(name, type, owner_id)
VALUES
    ('pluto', 'dog'::pet_type, 1),
    ('goofy', 'dog'::pet_type, 1),
    ('felix', 'cat'::pet_type, 2);";

        using var connection = new NpgsqlConnection(_ownerConnectionString);
        connection.Open();

        var command = new NpgsqlCommand(initDbCommandText, connection);
        command.ExecuteNonQuery();
    }

    [Test]
    public async Task GetTables_Ok()
    {
        var db = new PostgresDatabase(_ownerConnectionString);
        var result = await db.GetUsableTableNamesAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain("kids");
        result.Should().Contain("pets");
        
    }

    [Test]
    public async Task GetTableInfos_Ok()
    {
        var db = new PostgresDatabase(_ownerConnectionString);
        var result = await db.GetTableInfoAsync();

        result.Should().NotBeNullOrEmpty();
        result.Should().BeEquivalentTo(@"CREATE TABLE kids (
id	bigint NOT NULL,
name	text NOT NULL,
age	integer NOT NULL,
CHECK ((age > 0)),
PRIMARY KEY (id)
);

/*
3 rows from kids table:
id	name	age
1	jack	10
2	nancy	5
3	jimmy	7
*/
CREATE TABLE pets (
id	bigint NOT NULL,
name	text NOT NULL,
type	public.pet_type NOT NULL /*possible values cat,dog*/,
owner_id	bigint NOT NULL,
PRIMARY KEY (id),
FOREIGN KEY (owner_id) REFERENCES kids(id) ON DELETE CASCADE
);

/*
3 rows from pets table:
id	name	type	owner_id
1	pluto	dog	1
2	goofy	dog	1
3	felix	cat	2
*/
");
    }

    [Test]
    public async Task RunSql_FetchOne_Ok()
    {
        var db = new PostgresDatabase(_ownerConnectionString);
        var result = await db.RunAsync("SELECT name FROM kids ORDER BY id", SqlRunFetchType.One);

        result.Should().NotBeNullOrEmpty();
        result.Should().BeEquivalentTo("[(jack)]");
    }

    [Test]
    public async Task RunSql_FetchAll_Ok()
    {
        var db = new PostgresDatabase(_ownerConnectionString);
        var result = await db.RunAsync("SELECT name FROM kids ORDER BY id", SqlRunFetchType.All);
        
        result.Should().NotBeNullOrEmpty();
        result.Should().BeEquivalentTo("[(jack), (nancy), (jimmy)]");
    }

    [Test]
    public async Task SqlDatabaseChain_Run_Ok()
    {
        var key = Environment.GetEnvironmentVariable("OPENAI_KEY") ?? throw new ArgumentException("OPENAI_KEY");
        var llm = new OpenAiModel(
            new OpenAiConfiguration
            {
                ApiKey = key,
                ModelId = ChatModel.Gpt35Turbo,
                Temperature = 0.1
            });

        var llmInput = new LlmChainInput(llm, SqlDatabaseChainPrompts.PostgresPrompt);
        var llmChain = new LlmChain(llmInput);

        var db = new PostgresDatabase(_ownerConnectionString);
        var chainInput = new SqlDatabaseChainInput(db, llmChain);
        var chain = new SqlDatabaseChain(chainInput);

        var result = await chain.Run("How many kids have dogs?");

        result.Should().NotBeNullOrEmpty();
        Console.WriteLine(result);
        result.Should().Contain("1");
    }
}