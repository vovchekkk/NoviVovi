using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Novels;

public interface INovelRepository
{
    public Task<Novel?> GetByIdAsync(Guid id);
    public Task AddAsync(Novel novel);
    public Task DeleteAsync(Novel novel);
}