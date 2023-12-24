using LangChain.Abstractions.Schema;
using LangChain.Base;
using LangChain.Callback;
using LangChain.Chains.LLM;
using LangChain.Prompts;
using LangChain.Schema;

namespace LangChain.Utilities.Sql;

/// <summary>
/// Chain for interacting with SQL Database.
/// </summary>
/// <remarks>
/// Security note:
///     Make sure that the database connection uses credentials
///     that are narrowly-scoped to only include the permissions this chain needs.
///     Failure to do so may result in data corruption or loss, since this chain may
///     attempt commands like `DROP TABLE` or `INSERT` if appropriately prompted.
///     The best way to guard against such negative outcomes is to (as appropriate)
///     limit the permissions granted to the credentials used with this chain.
///     This issue shows an example negative outcome if these steps are not taken:
///     https://github.com/langchain-ai/langchain/issues/5923
/// </remarks>
public class SqlDatabaseChain(SqlDatabaseChainInput fields) : BaseChain(fields)
{
    /// <summary> Intermediate steps output key </summary>
    public const string IntermediateStepsKey = "intermediate_steps";

    /// <summary> Sql query checker prompt text </summary>
    public const string QueryChecker = @"
{query}
Double check the {dialect} query above for common mistakes, including:
- Using NOT IN with NULL values
- Using UNION when UNION ALL should have been used
- Using BETWEEN for exclusive ranges
- Data type mismatch in predicates
- Properly quoting identifiers
- Using the correct number of arguments for functions
- Casting to the correct data type
- Using the proper columns for joins

If there are any of the above mistakes, rewrite the query. If there are no mistakes, just reproduce the original query.

Output the final SQL query only.

SQL Query: ";

    /// <summary> Chain input fields </summary>
    private readonly SqlDatabaseChainInput _fields = fields;

    /// <inheritdoc />
    public override IReadOnlyList<string> InputKeys => new[] { _fields.InputKey };

    /// <inheritdoc />
    public override IReadOnlyList<string> OutputKeys => new[] { _fields.OutputKey };

    /// <inheritdoc />
    public override string ChainType() => "sql_database_chain";

    /// <inheritdoc />
    protected override async Task<IChainValues> CallAsync(IChainValues values, CallbackManagerForChainRun? runManager)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));
        runManager ??= BaseRunManager.GetNoopManager<CallbackManagerForChainRun>();

        var inputText = $"{values.Value[_fields.InputKey]}\nSQLQuery:";
        await runManager.HandleTextAsync(inputText).ConfigureAwait(false);

        var tableInfo = await _fields.SqlDatabase.GetTableInfoAsync(_fields.TableNamesToUse).ConfigureAwait(false);

        var llmInputs = new Dictionary<string, object>
        {
            ["input"] = inputText,
            ["top_k"] = _fields.TopK,
            ["dialect"] = _fields.SqlDatabase.Dialect,
            ["table_info"] = tableInfo,
            ["stop"] = new[] { "\nSQLResult:" }
        };

        // TODO: memory?
        // if (this.Memory != null)
        // {
        //     foreach(var k in this.Memory.MemoryVariables)
        //     { llmInputs[k] = values[k]; }

        var intermediateSteps = new List<IReadOnlyDictionary<string, object>>();
        try
        {
            intermediateSteps.Add(llmInputs);

            var sqlCmdRaw = await _fields.LlmChain.Predict(new ChainValues(llmInputs)).ConfigureAwait(false);
            var sqlCmd = ((string)sqlCmdRaw).Trim();

            if (_fields.ReturnSql)
            {
                return new ChainValues(_fields.OutputKey, sqlCmd);
            }

            string? result;
            if (!_fields.UseQueryChecker)
            {
                await runManager.HandleTextAsync(sqlCmd).ConfigureAwait(false);
                intermediateSteps.Add(new Dictionary<string, object> { ["sql_cmd"] = sqlCmd });
                result = await _fields.SqlDatabase.RunAsync(sqlCmd).ConfigureAwait(false);
                intermediateSteps.Add(new Dictionary<string, object> { ["sql_output"] = result });
            }
            else
            {
                var queryCheckerPrompt = _fields.QueryCheckerPrompt ?? PromptTemplate.FromTemplate(QueryChecker);
                var queryCheckerChain = new LlmChain(new LlmChainInput(_fields.LlmChain.Llm, queryCheckerPrompt));
                var queryCheckerInputs = new Dictionary<string, object>
                {
                    ["query"] = sqlCmd,
                    ["dialect"] = _fields.SqlDatabase.Dialect,
                };

                var checkedSqlCommandRaw = await queryCheckerChain
                    .Predict(new ChainValues(queryCheckerInputs))
                    .ConfigureAwait(false);

                var checkedSqlCommand = ((string)checkedSqlCommandRaw).Trim();

                await runManager.HandleTextAsync(checkedSqlCommand).ConfigureAwait(false);

                intermediateSteps.Add(new Dictionary<string, object> { ["sql_cmd_checked"] = checkedSqlCommand });
                result = await _fields.SqlDatabase.RunAsync(checkedSqlCommand).ConfigureAwait(false);
                intermediateSteps.Add(new Dictionary<string, object> { ["sql_output"] = result });
                sqlCmd = checkedSqlCommand;
            }

            await runManager.HandleTextAsync("\nSQLResult: ").ConfigureAwait(false);
            await runManager.HandleTextAsync(result).ConfigureAwait(false);

            // If return direct, we just set the final result equal to
            // the result of the sql query result, otherwise try to get a human readable
            // final answer
            string? finalResult;
            if (_fields.ReturnDirect)
            {
                finalResult = result;
            }
            else
            {
                await runManager.HandleTextAsync("\nAnswer:").ConfigureAwait(false);

                inputText += $"{sqlCmd}\nSQLResult: {result}\nAnswer:";
                llmInputs["input"] = inputText;
                intermediateSteps.Add(llmInputs);
                var finalResultRaw = await _fields.LlmChain.Predict(new ChainValues(llmInputs)).ConfigureAwait(false);
                finalResult = ((string)finalResultRaw).Trim();
                intermediateSteps.Add(new Dictionary<string, object> { ["final_result"] = finalResult });

                await runManager.HandleTextAsync(finalResult).ConfigureAwait(false);
            }

            var chainResult = new Dictionary<string, object> { [_fields.OutputKey] = finalResult };
            if (_fields.ReturnIntermediateSteps)
            {
                chainResult[IntermediateStepsKey] = intermediateSteps;
            }

            return new ChainValues(chainResult);
        }
        catch (Exception e)
        {
            e.Data["intermediate_steps"] = intermediateSteps;
            throw;
        }
    }
}