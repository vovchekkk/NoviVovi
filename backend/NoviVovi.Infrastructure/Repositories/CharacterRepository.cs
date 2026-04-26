using NoviVovi.Application.Characters.Abstactions;
using NoviVovi.Domain.Characters;
using NoviVovi.Infrastructure.Mappers;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

namespace NoviVovi.Infrastructure.Repositories;

public class CharacterRepository(ICharacterDbORepository dboRepo, CharacterMapper mapper) : ICharacterRepository
{
    public async Task<Character?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var dbo = await dboRepo.GetFullCharacterByIdAsync(id);
        if (dbo == null)
            return null;
        return mapper.ToDomain(dbo);
    }

    public async Task AddOrUpdateAsync(Character character, CancellationToken ct)
    {
        var dbo = mapper.ToDbO(character);
        await dboRepo.AddOrUpdateFullAsync(dbo);
    }

    public async Task DeleteAsync(Character character, CancellationToken ct)
    {
        await dboRepo.DeleteAsync(character.Id);
    }
}