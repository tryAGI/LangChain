using LangChain.Prompts;

namespace LangChain.Utilities.SqlDatabases;

/// <summary>
/// Prompts for SqlDatabaseChain
/// <remarks>
/// prompts from https://github.com/langchain-ai/langchain/blob/master/libs/langchain/langchain/chains/sql_database/prompt.py
/// </remarks> 
/// </summary>
public static class SqlDatabaseChainPrompts
{
    private const string PromptSuffixText =
        @"Only use the following tables:
{table_info}

Question: {input}";

    private const string DefaultTemplateText =
        @"Given an input question, first create a syntactically correct {dialect} query to run, then look at the results of the query and return the answer. Unless the user specifies in his question a specific number of examples he wishes to obtain, always limit your query to at most {top_k} results. You can order the results by a relevant column to return the most interesting examples in the database.

Never query for all the columns from a specific table, only ask for a the few relevant columns given the question.

Pay attention to use only the column names that you can see in the schema description. Be careful to not query for columns that do not exist. Also, pay attention to which column is in which table.

Use the following format:

Question: Question here
SQLQuery: SQL Query to run
SQLResult: Result of the SQLQuery
Answer: Final answer here

";

    private static readonly string[] _defaultPromptInputVariables = {"input", "table_info", "dialect", "top_k"};
    
    /// <summary>
    /// SqlDatabaseChain default prompt 
    /// </summary>
    public static PromptTemplate Default => new(
        new PromptTemplateInput(
            DefaultTemplateText + PromptSuffixText,
            _defaultPromptInputVariables));

    private const string PostgresPromptText =
        @"You are a PostgreSQL expert. Given an input question, first create a syntactically correct PostgreSQL query to run, then look at the results of the query and return the answer to the input question.
Unless the user specifies in the question a specific number of examples to obtain, query for at most {top_k} results using the LIMIT clause as per PostgreSQL. You can order the results to return the most informative data in the database.
Never query for all columns from a table. You must query only the columns that are needed to answer the question. Wrap each column name in double quotes ("") to denote them as delimited identifiers.
Pay attention to use only the column names you can see in the tables below. Be careful to not query for columns that do not exist. Also, pay attention to which column is in which table.
Pay attention to use CURRENT_DATE function to get the current date, if the question involves ""today"".

Use the following format:

Question: Question here
SQLQuery: SQL Query to run
SQLResult: Result of the SQLQuery
Answer: Final answer here

";

    private static readonly string[] _postgresPromptInputVariables = { "input", "table_info", "top_k" };

    /// <summary>
    /// Postgres SqlDatabaseChain default prompt
    /// </summary>
    public static PromptTemplate PostgresPrompt => new(
        new PromptTemplateInput(
            PostgresPromptText + PromptSuffixText,
            _postgresPromptInputVariables));

    
}
    