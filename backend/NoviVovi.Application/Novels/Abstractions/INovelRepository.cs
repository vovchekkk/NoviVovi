using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Novels.Abstractions;

public interface INovelRepository
{
    public Task<Novel?> GetByIdAsync(Guid id, CancellationToken ct);
    public Task AddOrUpdateAsync(Novel novel, CancellationToken ct);
    public Task DeleteAsync(Novel novel, CancellationToken ct);
    public Task<IEnumerable<Novel>> GetAllAsync(CancellationToken ct);
    
    Task<IEnumerable<Character>> GetAllCharactersAsync(Guid novelId, CancellationToken ct);
}