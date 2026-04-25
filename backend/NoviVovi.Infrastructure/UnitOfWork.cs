using System.Data;
using Npgsql;
using NoviVovi.Application.Common.Abstractions;

namespace NoviVovi.Infrastructure;

/// <summary>
/// Unit of Work implementation for Dapper with PostgreSQL.
/// Manages database connection and transaction lifecycle.
/// Should be registered as Scoped in DI container.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private IDbTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString);
        _connection.Open();
    }

    public IDbConnection Connection => _connection;
    public IDbTransaction? Transaction => _transaction;

    public void BeginTransaction()
    {
        if (_transaction != null)
            throw new InvalidOperationException("Transaction already started");
        
        _transaction = _connection.BeginTransaction();
    }

    public Task CommitAsync(CancellationToken ct = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to commit");

        try
        {
            _transaction.Commit();
        }
        catch
        {
            _transaction.Rollback();
            throw;
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }

        return Task.CompletedTask;
    }

    public Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to rollback");

        try
        {
            _transaction.Rollback();
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }

        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        // Alias for CommitAsync for compatibility with existing code
        return CommitAsync(ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _transaction?.Dispose();
        
        if (_connection is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync();
        else
            _connection?.Dispose();

        _disposed = true;
    }
}