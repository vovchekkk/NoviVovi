
using NoviVovi.Application.Novels;
using NoviVovi.Domain.Novels;
using NoviVovi.Infrastructure.Persistence;

namespace NoviVovi.Infrastructure.Novels;

public class NovelRepository(AppDbContext db) : INovelRepository
{
    public Task<Novel?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Novel novel, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Novel novel, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Novel>> GetAllAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}