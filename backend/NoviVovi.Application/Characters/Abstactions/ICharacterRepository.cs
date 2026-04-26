using NoviVovi.Domain.Characters;

namespace NoviVovi.Application.Characters.Abstactions;

public interface ICharacterRepository
{
    public Task<Character?> GetByIdAsync(Guid id, CancellationToken ct);
    public Task AddOrUpdateAsync(Character character, CancellationToken ct);
    public Task DeleteAsync(Character character, CancellationToken ct);
}