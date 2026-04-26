using System.Data;
using Dapper;
using NoviVovi.Application.Common.Abstractions;

namespace NoviVovi.Infrastructure.Repositories;

/// <summary>
/// Base repository for all DbO repositories.
/// Uses UnitOfWork for connection and transaction management.
/// All repositories share the same connection and transaction from UnitOfWork.
/// </summary>
public abstract class BaseRepository
{
    private readonly IUnitOfWork _unitOfWork;

    protected BaseRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Gets the database connection from UnitOfWork.
    /// All repositories use the same connection.
    /// </summary>
    protected IDbConnection Connection => _unitOfWork.Connection;

    /// <summary>
    /// Gets the current transaction from UnitOfWork.
    /// All repositories use the same transaction.
    /// </summary>
    protected IDbTransaction? Transaction => _unitOfWork.Transaction;

    protected async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        return await Connection.QueryFirstOrDefaultAsync<T>(
            new CommandDefinition(sql, param, Transaction, cancellationToken: ct)
        );
    }

    protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        return await Connection.QueryAsync<T>(
            new CommandDefinition(sql, param, Transaction, cancellationToken: ct)
        );
    }

    protected async Task<int> ExecuteAsync(string sql, object? param = null, CancellationToken ct = default)
    {
        return await Connection.ExecuteAsync(
            new CommandDefinition(sql, param, Transaction, cancellationToken: ct)
        );
    }

    protected async Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        return await Connection.ExecuteScalarAsync<T>(
            new CommandDefinition(sql, param, Transaction, cancellationToken: ct)
        );
    }
}