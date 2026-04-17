using NoviVovi.Infrastructure.DatabaseObjects.Novels;

namespace NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

public interface INovelDbORepository
{
    Task<NovelDbO?> GetFullByIdAsync(Guid id, LoadContext ctx);
    Task<IEnumerable<NovelDbO>> GetAllFullAsync(bool onlyPublic = true);
    Task DeleteAsync(Guid id);
    Task<Guid> AddOrUpdateFullAsync(NovelDbO novel);
}