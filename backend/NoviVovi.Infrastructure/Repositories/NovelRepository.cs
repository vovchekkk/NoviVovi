using NoviVovi.Application.Novels;
using NoviVovi.Domain.Novels;
using NoviVovi.Infrastructure.Mappers;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

namespace NoviVovi.Infrastructure.Repositories;

public class NovelRepository(INovelDbORepository dbORepository, NovelsMapper mapper) : INovelRepository
{
    public async Task<Novel?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var dbo = await dbORepository.GetFullByIdAsync(id);
        return dbo == null ? null : mapper.ToDomain(dbo);
    }

    public async Task AddAsync(Novel novel, CancellationToken ct)
    {
        var dbo = mapper.ToDbO(novel);
        await dbORepository.AddFullAsync(dbo);
    }

    public async Task DeleteAsync(Novel novel, CancellationToken ct)
    {
        await dbORepository.DeleteAsync(novel.Id);
    }

    public async Task<IEnumerable<Novel>> GetAllAsync(CancellationToken ct)
    {
        var dbos = await dbORepository.GetAllFullAsync();
        return dbos.Select(dto => mapper.ToDomain(dto));
    }
}