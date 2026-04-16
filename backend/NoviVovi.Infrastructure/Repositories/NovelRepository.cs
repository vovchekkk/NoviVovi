using NoviVovi.Application.Novels;
using NoviVovi.Domain.Characters;
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

    public Task<Character?> GetCharacterByIdAsync(Guid characterId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task AddCharacterAsync(Character character, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCharacterAsync(Character character, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Character>> GetAllCharactersAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<CharacterState?> GetCharacterStateByIdAsync(Guid characterId, Guid stateId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task AddCharacterStateAsync(Guid characterId, CharacterState state, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCharacterStateAsync(Guid characterId, CharacterState state, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CharacterState>> GetAllCharacterStatesAsync(Guid characterId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}