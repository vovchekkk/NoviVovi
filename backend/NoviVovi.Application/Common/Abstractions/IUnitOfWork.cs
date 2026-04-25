using System.Data;

namespace NoviVovi.Application.Common.Abstractions;

/// <summary>
/// Unit of Work pattern for Dapper.
/// Manages database connection and transaction lifecycle.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Gets the database connection.
    /// All repositories should use this connection.
    /// </summary>
    IDbConnection Connection { get; }
    
    /// <summary>
    /// Gets the current transaction.
    /// All repositories should use this transaction.
    /// </summary>
    IDbTransaction? Transaction { get; }
    
    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    void BeginTransaction();
    
    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    Task CommitAsync(CancellationToken ct = default);
    
    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    Task RollbackAsync(CancellationToken ct = default);
    
    /// <summary>
    /// Saves changes (commits transaction).
    /// Alias for CommitAsync for compatibility.
    /// </summary>
    Task SaveChangesAsync(CancellationToken ct);
}