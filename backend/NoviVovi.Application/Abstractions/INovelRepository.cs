using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Abstractions;

public interface INovelRepository
{
    public Task<Novel?> GetById(Guid id);
    public Task Save(Novel novel);
    public Task Delete(Novel novel);
}