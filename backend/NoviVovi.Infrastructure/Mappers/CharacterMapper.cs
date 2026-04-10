using NoviVovi.Domain.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class CharacterMapper(CharacterStateMapper mapper)
{
    public Character ToCharacter(CharacterDbO dbo)
    {
        var res = new Character(dbo.Id, dbo.Name, dbo.Description);
        foreach (var state in dbo.States)
            res.AddState(mapper.ToDomain(state));
        return res;
    }

    public CharacterDbO ToDbO(Character character, Guid novelId)
    {
        var res = new CharacterDbO
        {
            Id = character.Id,
            Name = character.Name,
            NovelId = novelId,
            Description = character.Description,
            States = mapper.ToDbO(character.CharacterStates, character.Id)
        };
        return res;
    }
}