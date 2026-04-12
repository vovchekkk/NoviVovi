using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Novels;

public interface INovelRepository
{
    public Task<Novel?> GetByIdAsync(Guid id, CancellationToken ct);
    public Task AddAsync(Novel novel, CancellationToken ct);
    public Task DeleteAsync(Novel novel, CancellationToken ct);
    public Task<IEnumerable<Novel>> GetAllAsync(CancellationToken ct);
    
    public Task<Character?> GetCharacterByIdAsync(Guid characterId, CancellationToken ct);
    public Task AddCharacterAsync(Character character, CancellationToken ct);
    public Task DeleteCharacterAsync(Character character, CancellationToken ct);
    public Task<IEnumerable<Character>> GetAllCharactersAsync(CancellationToken ct);
    
    public Task<CharacterState?> GetCharacterStateByIdAsync(Guid characterId, Guid stateId, CancellationToken ct);
    public Task AddCharacterStateAsync(Guid characterId, CharacterState state, CancellationToken ct);
    public Task DeleteCharacterStateAsync(Guid characterId, CharacterState state, CancellationToken ct);
    public Task<IEnumerable<CharacterState>> GetAllCharacterStatesAsync(Guid characterId, CancellationToken ct);
}