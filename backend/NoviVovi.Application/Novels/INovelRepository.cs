using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Novels;

public interface INovelRepository
{
    public Task<Novel?> GetByIdAsync(Guid id, CancellationToken ct);
    public Task AddAsync(Novel novel, CancellationToken ct);
    public Task DeleteAsync(Novel novel, CancellationToken ct);
    public Task<IEnumerable<Novel>> GetAllAsync(CancellationToken ct);
}