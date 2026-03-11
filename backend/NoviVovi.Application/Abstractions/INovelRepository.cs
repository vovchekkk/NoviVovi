using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Abstractions;

public interface INovelRepository
{
    public Task<Novel?> GetByIdAsync(Guid id);
    public Task SaveAsync(Novel novel);
    public Task DeleteAsync(Novel novel);
}