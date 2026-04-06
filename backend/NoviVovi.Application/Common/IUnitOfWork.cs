namespace NoviVovi.Application.Common;

public interface IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct);
}