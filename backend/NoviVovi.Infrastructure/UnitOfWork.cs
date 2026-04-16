using NoviVovi.Application.Common;

namespace NoviVovi.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}