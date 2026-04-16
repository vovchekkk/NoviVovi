using System.Data;
using Dapper;
using Npgsql;

namespace NoviVovi.Infrastructure.Repositories;

public abstract class BaseRepository : IDisposable
{
    private readonly string connectionString;
    private NpgsqlConnection? connection;
    private NpgsqlTransaction? transaction;

    protected BaseRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    protected async Task<NpgsqlConnection> GetConnectionAsync()
    {
        if (connection == null || connection.State != ConnectionState.Open)
        {
            connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
        }
        return connection;
    }

    protected async Task<NpgsqlTransaction> BeginTransactionAsync()
    {
        var conn = await GetConnectionAsync();
        transaction = await conn.BeginTransactionAsync();
        return transaction;
    }

    protected async Task CommitTransactionAsync()
    {
        if (transaction != null)
        {
            await transaction.CommitAsync();
            await transaction.DisposeAsync();
            transaction = null;
        }
    }

    protected async Task RollbackTransactionAsync()
    {
        if (transaction != null)
        {
            await transaction.RollbackAsync();
            await transaction.DisposeAsync();
            transaction = null;
        }
    }

    protected async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null)
    {
        await using var conn = await GetConnectionAsync();
        return await conn.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
    }

    protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
    {
        await using var conn = await GetConnectionAsync();
        return await conn.QueryAsync<T>(sql, param, transaction);
    }

    protected async Task<int> ExecuteAsync(string sql, object? param = null)
    {
        await using var conn = await GetConnectionAsync();
        return await conn.ExecuteAsync(sql, param, transaction);
    }

    protected async Task<Guid?> ExecuteScalarAsync<Guid>(string sql, object? param = null)
    {
        await using var conn = await GetConnectionAsync();
        return await conn.ExecuteScalarAsync<Guid>(sql, param, transaction);
    }

    public void Dispose()
    {
        transaction?.Dispose();
        connection?.Dispose();
    }
}