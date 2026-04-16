using System.Data;
using Dapper;
using Npgsql;

namespace NoviVovi.Infrastructure.Repositories;

// Мы используем IAsyncDisposable, так как работаем с асинхронными ресурсами БД
public abstract class BaseRepository(DatabaseOptions options) : IDisposable, IAsyncDisposable
{
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;

    protected async Task<NpgsqlConnection> GetConnectionAsync(CancellationToken ct = default)
    {
        // 1. Используем строку из объекта настроек, который мы внедрили через DI
        if (_connection == null)
        {
            _connection = new NpgsqlConnection(options.ConnectionString);
        }

        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync(ct);
        }

        return _connection;
    }

    protected async Task<NpgsqlTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        var conn = await GetConnectionAsync(ct);
        _transaction = await conn.BeginTransactionAsync(ct);
        return _transaction;
    }

    protected async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    protected async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    // ВАЖНО: Мы НЕ используем 'await using' внутри методов, 
    // так как соединением управляет жизненный цикл самого репозитория (Scoped)
    protected async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        var conn = await GetConnectionAsync(ct);
        return await conn.QueryFirstOrDefaultAsync<T>(new CommandDefinition(sql, param, _transaction, cancellationToken: ct));
    }

    protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        var conn = await GetConnectionAsync(ct);
        return await conn.QueryAsync<T>(new CommandDefinition(sql, param, _transaction, cancellationToken: ct));
    }

    protected async Task<int> ExecuteAsync(string sql, object? param = null, CancellationToken ct = default)
    {
        var conn = await GetConnectionAsync(ct);
        return await conn.ExecuteAsync(new CommandDefinition(sql, param, _transaction, cancellationToken: ct));
    }

    // Исправил generic-параметр: не называй его <Guid>, чтобы не путать с типом Guid
    protected async Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        var conn = await GetConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<T>(new CommandDefinition(sql, param, _transaction, cancellationToken: ct));
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null) await _transaction.DisposeAsync();
        if (_connection != null) await _connection.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}