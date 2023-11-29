using LangChain.Base;
using LangChain.Chains.LLM;
using LangChain.Prompts.Base;

namespace LangChain.Utilities.Sql;

/// <summary>
/// 
/// </summary>
public sealed class SqlDatabaseChainInput(SqlDatabase sqlDatabase, LlmChain llmChain) : ChainInputs
{
    /// <summary>
    /// Database to fetch info from
    /// </summary>
    public SqlDatabase SqlDatabase { get; } = sqlDatabase;

    /// <summary>
    /// LLM chain for query generation
    /// </summary>
    public LlmChain LlmChain { get; set; } = llmChain;

    /// <summary>
    /// Chain input key
    /// </summary>
    public string InputKey { get; set; } = "query";

    /// <summary>
    /// Chain output key
    /// </summary>
    public string OutputKey { get; set; } = "result";

    /// <summary>
    /// Will return sql-command directly without executing it
    /// </summary>
    public bool ReturnSql { get; set; }

    /// <summary>
    /// Whether or not to return intermediate steps
    /// </summary>
    public bool ReturnIntermediateSteps { get; set; }

    /// <summary>
    /// Number of results to return from the query
    /// </summary>
    public int TopK { get; set; } = 5;

    /// <summary>
    /// Whether or not to return the result of querying the SQL table directly.
    /// </summary>
    public bool ReturnDirect { get; set; }

    /// <summary>
    /// Whether or not the query checker tool should be used to attempt
    /// to fix the initial SQL from the LLM.
    /// </summary>
    public bool UseQueryChecker { get; set; }

    /// <summary>
    /// The prompt template that should be used by the query checker
    /// </summary>
    public BasePromptTemplate? QueryCheckerPrompt { get; set; }

    /// <summary>
    /// If not present, then defaults to null which is all tables.
    /// </summary>
    public IReadOnlyList<string>? TableNamesToUse { get; }
}