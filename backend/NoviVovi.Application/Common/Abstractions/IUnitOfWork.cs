namespace NoviVovi.Application.Common.Abstractions;

public interface IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct);
}