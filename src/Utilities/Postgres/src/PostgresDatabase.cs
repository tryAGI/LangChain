using System.Data;
using System.Text;
using LangChain.Utilities.Sql;
using Npgsql;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace LangChain.Utilities.Postgres;

/// <summary>
/// Postgres implementation of <see cref="SqlDatabase"/>
/// </summary>
public sealed class PostgresDatabase : SqlDatabase
{
    private readonly NpgsqlDataSource _dataSource;

    /// <inheritdoc />
    public override string Dialect => "postgresql";

    /// <inheritdoc />
    public PostgresDatabase(
        string connectionString,
        string? schema = null,
        IReadOnlyList<string>? includeTables = null,
        IReadOnlyList<string>? ignoreTables = null,
        int sampleRowsInTableInfo = 3,
        bool indexesInTableInfo = false,
        IReadOnlyDictionary<string, string>? customTableInfo = null,
        bool viewSupport = false,
        int maxStringLength = 300)
        : base(
            schema, includeTables, ignoreTables,
            sampleRowsInTableInfo, indexesInTableInfo, customTableInfo,
            viewSupport, maxStringLength)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        _dataSource = dataSourceBuilder.Build();
    }

    /// <inheritdoc />
    protected override async Task<List<string>> GetAllTableNamesAsync()
    {
        using var connection = await _dataSource.OpenConnectionAsync().ConfigureAwait(false);
        var tablesSchema = await connection.GetSchemaAsync("TABLES", new[] { String.Empty, "public" })
            .ConfigureAwait(false);

        var tables = new List<string>();
        foreach (DataRow row in tablesSchema.Rows)
        {
            var tableName = row["table_name"].ToString();
            var tableType = row["table_type"].ToString();
            var tableSchema = row["table_schema"].ToString();

            if (tableName != null && tableSchema != null && tableType == "BASE TABLE")
            {
                tables.Add(tableName);
            }
        }

        return tables;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override Task HasOnlyReadPrivilegesAsync()
    {
        // e.g. for postres
        // SELECT DISTINCT privilege_type
        // FROM   information_schema.table_privileges
        // WHERE  grantee = CURRENT_USER
        // possible values https://www.postgresql.org/docs/current/ddl-priv.html
        // throw if not SELECT only or some flag passed to constructor?

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override async Task<List<Dictionary<string, object>>> ExecuteAsync(string sql, SqlRunFetchType fetch)
    {
        using var connection = await _dataSource.OpenConnectionAsync().ConfigureAwait(false);

        if (!String.IsNullOrEmpty(Schema))
        {
            using var setSchemaCommand = new NpgsqlCommand($"SET search_path TO {Schema};", connection);
            await setSchemaCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        using var command = new NpgsqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

        var result = new List<Dictionary<string, object>>();
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            var row = Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue);
            result.Add(row);

            if (fetch == SqlRunFetchType.One)
            {
                break;
            }
        }

        return result;
    }

    /// <inheritdoc />
    protected override async Task<string> GetCreateTableScriptAsync(string table)
    {
        var columns = await GetColumnsAsync(table).ConfigureAwait(false);
        var builder = new StringBuilder();

        builder.AppendLine($"CREATE TABLE {table} (");

        builder.AppendJoin(
            ",\n",
            columns.Select(column =>
            {
                var nullable = column.IsNullable ? "" : " NOT NULL";
                var dataType = column.DataType;
                var comment = !String.IsNullOrEmpty(column.Comment) ? $" /*{column.Comment}*/" : null;
                return $"{column.Name}\t{dataType}{nullable}{comment}";
            }));

        var constraintsAndIndexes = await GetTableConstraintAndKeysAsync(table).ConfigureAwait(false);
        if (constraintsAndIndexes.Count > 0)
        {
            builder.Append(",\n");
            builder.AppendJoin(",\n", constraintsAndIndexes.Select(c => c.Definition));
        }

        builder.AppendLine("\n);");

        return builder.ToString();
    }

    /// <inheritdoc />
    protected override async Task<string> GetSampleRowsAsync(string table)
    {
        using var connection = await _dataSource.OpenConnectionAsync().ConfigureAwait(false);
        var tableWithSchema = String.IsNullOrEmpty(Schema) ? table : Schema + "." + table;
        using var command = new NpgsqlCommand($"SELECT * FROM {tableWithSchema}", connection);
        using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

        var columnsSchema = await reader.GetColumnSchemaAsync().ConfigureAwait(false);
        var headers = String.Join("\t", columnsSchema.Select(c => c.ColumnName));

        string? sampleRows = null;
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            var samples = new List<string>(columnsSchema.Count);
            foreach (var column in columnsSchema)
            {
                if (column.ColumnOrdinal == null)
                {
                    throw new ArgumentException(
                        $"column {column.ColumnName} in {table} has null {nameof(column.ColumnOrdinal)}");
                }

                var sample = reader.GetValue(column.ColumnOrdinal.Value).ToString() ?? string.Empty;
                samples.Add(sample.Length > 100 ? sample.Substring(0, 100) : sample);
            }

            if (sampleRows != null)
            {
                sampleRows += "\n";
            }

            sampleRows += String.Join("\t", samples);
        }

        return $"{SampleRowsInTableInfo} rows from {table} table:\n{headers}\n{sampleRows}";
    }

    private sealed record PgColumn(string Name, string DataType, bool IsNullable, string? Comment);

    private async Task<List<PgColumn>> GetColumnsAsync(string table)
    {
        using var connection = await _dataSource.OpenConnectionAsync().ConfigureAwait(false);

        var tableWithSchema = String.IsNullOrEmpty(Schema) ? table : Schema + "." + table;
        using var command = new NpgsqlCommand($"SELECT * FROM {tableWithSchema}", connection);
        using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

        var columnsSchema = await reader.GetColumnSchemaAsync().ConfigureAwait(false);
        var columns = new List<PgColumn>();
        foreach (var columnSchema in columnsSchema)
        {
            var columnName = columnSchema.ColumnName;
            var dataType = columnSchema.DataTypeName;
            var isNullable = columnSchema.AllowDBNull == true;
            string? comment = null;
            if (columnSchema.PostgresType is PostgresEnumType postgresEnumType)
            {
                comment = $"possible values {String.Join(",", postgresEnumType.Labels)}";
            }

            columns.Add(new PgColumn(columnName, dataType, isNullable, comment));
        }

        return columns;
    }

    private sealed record PgConstraint(string Name, string Definition);

    private async Task<List<PgConstraint>> GetTableConstraintAndKeysAsync(string table)
    {
        using var connection = await _dataSource.OpenConnectionAsync().ConfigureAwait(false);

        var constraints = new List<PgConstraint>();
        using var command = new NpgsqlCommand(@"
SELECT
    conname,
    pg_get_constraintdef(oid) as condef
FROM pg_catalog.pg_constraint r
WHERE @schema IS NOT NULL AND conrelid = (@schema || '.'  || @table)::regclass
    OR conrelid = @table::regclass;",
            connection);

        command.Parameters.AddWithValue("schema", NpgsqlDbType.Text, (object?)Schema ?? DBNull.Value);
        command.Parameters.AddWithValue("table", NpgsqlDbType.Text, table);

        using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            var name = reader.GetString(reader.GetOrdinal("conname"));
            var definition = reader.GetString(reader.GetOrdinal("condef"));

            constraints.Add(new PgConstraint(name, definition));
        }

        return constraints;
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _dataSource.Dispose();
    }
}