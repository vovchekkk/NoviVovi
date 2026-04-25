using NoviVovi.Application.Novels;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Novels;
using NoviVovi.Infrastructure.Mappers;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

namespace NoviVovi.Infrastructure.Repositories;

public class NovelRepository(INovelDbORepository dbORepository, ICharacterDbORepository characterDbORepo, NovelMapper mapper, CharacterMapper characterMapper) : INovelRepository
{
    public async Task<Novel?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var ctx = new LoadContext();
        var dbo = await dbORepository.GetFullByIdAsync(id, ctx);
        return dbo == null ? null : mapper.ToDomain(dbo);
    }

    public async Task AddOrUpdateAsync(Novel novel, CancellationToken ct)
    {
        var dbo = mapper.ToDbO(novel);
        await dbORepository.AddOrUpdateFullAsync(dbo);
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
    
    public async Task<IEnumerable<Character>> GetAllCharactersAsync(Guid novelId, CancellationToken ct)
    {
        var dbos = await characterDbORepo.GetFullByNovelIdAsync(novelId);
        return dbos.Where(dbo => dbo != null).Select(characterMapper.ToDomain);
    }
}