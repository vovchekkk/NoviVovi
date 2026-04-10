using NoviVovi.Infrastructure.DatabaseObjects.Novels;

namespace NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

public interface INovelDbORepository
{
    Task<NovelDbO?> GetFullByIdAsync(Guid id);
    Task<IEnumerable<NovelDbO>> GetAllFullAsync(bool onlyPublic = true);
    Task<Guid> AddAsync(NovelDbO novel);
    Task UpdateAsync(NovelDbO novel);
    Task DeleteAsync(Guid id);
    Task AddFullAsync(NovelDbO dbo);
}