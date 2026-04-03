using NoviVovi.Domain.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class CharacterMapper
{
    public partial Character ToCharacter(CharacterDbO dbo); //добавить в character цвет имени

    public partial CharacterDbO ToDbO(Character character, Guid novelId); //бля, а как новеллу восстановить?

    public FullCharacterDbO ToFullDbO(Character character, Guid novelId)
    {
        var stateMapper = new CharacterStateMapper();
        var res = new FullCharacterDbO
        {
            Character = ToDbO(character, novelId),
            States = stateMapper.ToDbO(character.CharacterStates, character.Id)
        };
        return res;
    }
}