using NoviVovi.Infrastructure.DatabaseObjects.Characters;

namespace NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

public interface ICharacterDbORepository
{
    Task<IEnumerable<CharacterDbO>> GetFullByNovelIdsAsync(IEnumerable<Guid> novelIds);
    Task<CharacterStateDbO?> GetFullCharacterStateByIdAsync(Guid stateId);
    Task<IEnumerable<CharacterDbO?>> GetFullByNovelIdAsync(Guid id);
    Task<CharacterDbO?> GetFullCharacterByIdAsync(Guid id);

    Task<Guid> AddAsync(CharacterDbO character);
    Task UpdateAsync(CharacterDbO character);
    Task DeleteAsync(Guid id);
    Task<Guid> AddOrUpdateStepCharacterAsync(StepCharacterDbO stepCharacter);
    Task<Guid> AddOrUpdateFullAsync(CharacterDbO character);
    Task<StepCharacterDbO?> GetCharacterObjectByCharacterIdAsync(Guid stepCharacterId);
    Task<Guid> AddOrUpdateStateAsync(CharacterStateDbO state);
    Task DeleteStateAsync(Guid stateId);
    Task DeleteStepCharacterAsync(Guid stepCharId);
}
