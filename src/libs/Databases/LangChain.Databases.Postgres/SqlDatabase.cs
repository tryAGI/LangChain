using System.Collections.Immutable;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace LangChain.Databases;

/// <summary>
/// 
/// </summary>
public abstract class SqlDatabase : IDisposable
{
    /// <summary> </summary>
    protected string? Schema { get; }

    /// <summary> </summary>
    protected IReadOnlyList<string>? IgnoreTables { get; }

    /// <summary> </summary>
    protected IReadOnlyList<string>? IncludeTables { get; }

    /// <summary> </summary>
    protected int SampleRowsInTableInfo { get; }

    /// <summary> </summary>
    protected bool IndexesInTableInfo { get; }

    /// <summary> </summary>
    protected IReadOnlyDictionary<string, string>? CustomTableInfo { get; }

    /// <summary> </summary>
    public bool ViewSupport { get; }

    /// <summary> </summary>
    public int MaxStringLength { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="schema">schema to use</param>
    /// <param name="includeTables">tables to include</param>
    /// <param name="ignoreTables">list of tables to ignore</param>
    /// <param name="sampleRowsInTableInfo"></param>
    /// <param name="indexesInTableInfo"></param>
    /// <param name="customTableInfo"></param>
    /// <param name="viewSupport"></param>
    /// <param name="maxStringLength"></param>
    protected SqlDatabase(
        string? schema = null,
        IReadOnlyList<string>? includeTables = null,
        IReadOnlyList<string>? ignoreTables = null,
        int sampleRowsInTableInfo = 3,
        bool indexesInTableInfo = false,
        IReadOnlyDictionary<string, string>? customTableInfo = null,
        bool viewSupport = false,
        int maxStringLength = 300)
    {
        CheckSchema(schema);
        Schema = schema;

        CheckPrivileges();

        IgnoreTables = ignoreTables;
        IncludeTables = includeTables;
        SampleRowsInTableInfo = sampleRowsInTableInfo;
        IndexesInTableInfo = indexesInTableInfo;
        CustomTableInfo = customTableInfo;
        ViewSupport = viewSupport;
        MaxStringLength = maxStringLength;
    }

    private static void CheckSchema(string? schema)
    {
        if (schema == null)
        {
            return;
        }

        if (!Regex.IsMatch(schema, "^[A-z0-0_]*$"))
        {
            throw new ArgumentException("Schema name violation");
        }
    }

    // TODO: Check readonly privileges or if some flag set
    private void CheckPrivileges()
    {
        // e.g. for postres
        // SELECT DISTINCT privilege_type
        // FROM   information_schema.table_privileges
        // WHERE  grantee = CURRENT_USER
        // possible values https://www.postgresql.org/docs/current/ddl-priv.html
        // throw if not SELECT only or some flag passed to constructor?
    }

    /// <summary>
    /// Execute a SQL command and return a dictionary representing the results.
    /// </summary>
    protected abstract Task<List<Dictionary<string, object>>> ExecuteAsync(string sql, SqlRunFetchType fetch);

    /// <summary>
    /// Return string representation of dialect to use.
    /// </summary>
    public abstract string Dialect { get; }

    /// <summary>
    /// TODO: ViewSupport
    /// Get names of tables available.
    /// </summary>
    public async Task<IEnumerable<string>> GetUsableTableNamesAsync()
    {
        var tables = await GetAllTableNamesAsync().ConfigureAwait(false);

        if (IncludeTables != null && IncludeTables.Count > 0)
        {
            return IncludeTables;
        }

        return tables.Where(t => IgnoreTables == null || !IgnoreTables.Contains(t));
    }

    /// <summary>
    /// TODO: support IndexesInTableInfo
    /// Get information about specified tables.
    /// </summary>
    /// <param name="tableNames"></param>
    /// <returns></returns>
    public async Task<string> GetTableInfoAsync(IReadOnlyList<string>? tableNames = null)
    {
        var allTableNames = await GetUsableTableNamesAsync().ConfigureAwait(false);
        var allTableNamesFiltered = new HashSet<string>().ToImmutableHashSet();
        if (tableNames != null && tableNames.Count != 0)
        {
            var tablesHashSet = new HashSet<string>(tableNames);
            tablesHashSet.IntersectWith(allTableNames);
            if (tablesHashSet.Count != tableNames.Count)
            {
                throw new ArgumentException("table_names {missing_tables} not found in database");
            }

            allTableNamesFiltered = tableNames.ToImmutableHashSet();
        }

        var builder = new StringBuilder();

        foreach (var table in allTableNamesFiltered)
        {
            var createScript = await GetCreateTableScriptAsync(table).ConfigureAwait(false);
            builder.AppendLine(createScript);

            var hasExtraInfo = SampleRowsInTableInfo > 0 || IndexesInTableInfo;
            if (hasExtraInfo)
            {
                builder.AppendLine("/*");
            }

            if (SampleRowsInTableInfo > 0)
            {
                var sample = await GetSampleRowsAsync(table).ConfigureAwait(false);
                builder.AppendLine(sample);
            }

            if (hasExtraInfo)
            {
                builder.AppendLine("*/");
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// Get create script for table
    /// </summary>
    protected abstract Task<string> GetCreateTableScriptAsync(string table);

    /// <summary>
    /// Get sample rows for table
    /// </summary>
    protected abstract Task<string> GetSampleRowsAsync(string table);

    /// <summary>
    /// Get all table names
    /// </summary>
    /// <returns></returns>
    protected abstract Task<List<string>> GetAllTableNamesAsync();

    /// <summary>
    /// Execute a SQL command and return a string representing the results.
    /// 
    /// If the statement returns rows, a string of the results is returned.
    /// If the statement returns no rows, an empty string is returned.
    /// </summary>
    public async Task<string> RunAsync(string command, SqlRunFetchType fetch = SqlRunFetchType.All)
    {
        var result = await ExecuteAsync(command, fetch).ConfigureAwait(false);

        var res = result
            .Select(r => r.Values.Select(v => StringHelperer.TruncateWord(v, MaxStringLength)))
            .Select(r => $"({string.Join(",", r)})");

        if (res.Any())
        {
            return $"[{String.Join(", ", res)}]";
        }

        return "";
    }

    /// <summary>
    /// Get information about specified tables.
    /// </summary>
    /// <param name="tableNames"></param>
    /// <returns></returns>
    public async Task<string> GetTableInfoNoThrowAsync(IReadOnlyList<string>? tableNames = null)
    {
        try
        {
            var result = await GetTableInfoAsync(tableNames).ConfigureAwait(false);

            return result;
        }
        catch (DbException e)
        {
            return $"Error: {e}";
        }
    }

    /// <summary>
    /// Execute a SQL command and return a string representing the results.
    /// 
    /// If the statement returns rows, a string of the results is returned.
    /// If the statement returns no rows, an empty string is returned.
    /// </summary>
    /// <param name="command">sql</param>
    /// <param name="fetch">fetch type</param>
    /// <returns></returns>
    public async Task<string> RunNoThrowAsync(string command, SqlRunFetchType fetch = SqlRunFetchType.All)
    {
        try
        {
            var result = await RunAsync(command, fetch).ConfigureAwait(false);

            return result;
        }
        catch (DbException e)
        {
            return $"Error: {e}";
        }
    }

    /// <inheritdoc />
    public virtual void Dispose()
    {
    }
}