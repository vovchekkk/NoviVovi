using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Novels;

public interface INovelRepository
{
    public Task<Novel?> GetByIdAsync(Guid id, CancellationToken ct);
    public Task AddAsync(Novel novel, CancellationToken ct);
    public Task DeleteAsync(Novel novel, CancellationToken ct);
    public Task<IEnumerable<Novel>> GetAllAsync(CancellationToken ct);
    
    public Task<Character?> GetCharacterByIdAsync(Guid novelId, Guid characterId, CancellationToken ct);
    public Task AddCharacterAsync(Guid novelId, Character character, CancellationToken ct);
    public Task DeleteCharacterAsync(Guid novelId, Character character, CancellationToken ct);
    public Task<IEnumerable<Character>> GetAllCharactersAsync(Guid novelId, CancellationToken ct);
    
    public Task<CharacterState?> GetCharacterStateByIdAsync(Guid novelId, Guid characterId, Guid stateId, CancellationToken ct);
    public Task AddCharacterStateAsync(Guid novelId, Guid characterId, CharacterState state, CancellationToken ct);
    public Task DeleteCharacterStateAsync(Guid novelId, Guid characterId, CharacterState state, CancellationToken ct);
    public Task<IEnumerable<CharacterState>> GetAllCharacterStatesAsync(Guid novelId, Guid characterId, CancellationToken ct);
}