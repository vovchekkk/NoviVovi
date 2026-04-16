using NoviVovi.Application.Novels;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Infrastructure.Novels;

public class NovelRepository() : INovelRepository
{
    public Task<Novel?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Novel novel, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Novel novel, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Novel>> GetAllAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Character?> GetCharacterByIdAsync(Guid novelId, Guid characterId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task AddCharacterAsync(Guid novelId, Character character, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCharacterAsync(Guid novelId, Character character, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Character>> GetAllCharactersAsync(Guid novelId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<CharacterState?> GetCharacterStateByIdAsync(Guid novelId, Guid characterId, Guid stateId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task AddCharacterStateAsync(Guid novelId, Guid characterId, CharacterState state, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCharacterStateAsync(Guid novelId, Guid characterId, CharacterState state, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CharacterState>> GetAllCharacterStatesAsync(Guid novelId, Guid characterId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}