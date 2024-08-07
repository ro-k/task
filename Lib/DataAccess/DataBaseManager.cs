using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Npgsql;
using Dapper;
using Lib.Models;
using Microsoft.Extensions.Options;

namespace Lib.DataAccess;

public class DataBaseManager : IDataBaseManager
{
    private readonly Func<IDbConnection> _connectionFactory;
    
    [ExcludeFromCodeCoverage]
    public DataBaseManager(IOptions<AppSettings> appSettings)
    {
        _connectionFactory = () => new NpgsqlConnection(appSettings.Value.ConnectionString!);
    }
    
    // For unit testing
    public DataBaseManager(Func<IDbConnection> connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null)
    {
        using var dbConnection = _connectionFactory();
        dbConnection.Open();
        return await dbConnection.QueryAsync<T>(query, parameters);
    }

    public async Task<T> QuerySingleOrDefaultAsync<T>(string query, object? parameters = null)
    {
        using var dbConnection = _connectionFactory();
        dbConnection.Open();
        return (await dbConnection.QuerySingleOrDefaultAsync<T>(query, parameters))!;
    }
    
    public async Task<T> QueryFirstOrDefaultAsync<T>(string query, object? parameters = null)
    {
        using var dbConnection = _connectionFactory();
        dbConnection.Open();
        return (await dbConnection.QueryFirstOrDefaultAsync<T>(query, parameters))!;
    }
    
    public async Task<int> ExecuteAsync(string query, object? parameters = null)
    {
        using var dbConnection = _connectionFactory();
        dbConnection.Open();
        return await dbConnection.ExecuteAsync(query, parameters);
    }

    [ExcludeFromCodeCoverage(Justification = "ExecuteScalarAsync does not have mocking support")]
    public async Task<T> ExecuteScalarAsync<T>(string query, object? parameters = null)
    {
        using var dbConnection = _connectionFactory();
        dbConnection.Open();
        return (await dbConnection.ExecuteScalarAsync<T>(query, parameters))!;
    }
    
    [ExcludeFromCodeCoverage(Justification = "QueryMultipleAsync does not have mocking support")]
    public async Task<T> QueryMultipleAsync<T>(Func<SqlMapper.GridReader, Task<T>> processGridReader, string query, object? parameters = null)
    {
        using var dbConnection = _connectionFactory();
        dbConnection.Open();
        await using var gridReader = await dbConnection.QueryMultipleAsync(query, parameters);
        return await processGridReader(gridReader);
    }

    public (string, DynamicParameters) WrapQueryWithConcurrencyCheck(string query, IHasConcurrencyStamp baseParameters)
    {
        var whereRegex = new Regex(@"\bWHERE\b", RegexOptions.IgnoreCase);
        var oldConcurrencyStamp = baseParameters.ConcurrencyStamp;
        baseParameters.ConcurrencyStamp = Guid.NewGuid();
        
        var parameters = new DynamicParameters(baseParameters);
        parameters.Add("@OldConcurrencyStamp", oldConcurrencyStamp.ToString());
        parameters.RemoveUnused = false;

        var appendedQuery = whereRegex.IsMatch(query)
            ? Regex.Replace(query, @"\bWHERE\b", $"WHERE concurrency_stamp = @OldConcurrencyStamp AND",
                RegexOptions.IgnoreCase)
            : $"{query} WHERE concurrency_stamp = @OldConcurrencyStamp";

        appendedQuery = appendedQuery.TrimEnd(' ', ';');

        appendedQuery = @$"
{appendedQuery}
RETURNING *;
";

        return (appendedQuery, parameters);
    }
}
