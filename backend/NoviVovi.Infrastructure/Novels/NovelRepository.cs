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