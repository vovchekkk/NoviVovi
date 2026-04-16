using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;

namespace NoviVovi.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}