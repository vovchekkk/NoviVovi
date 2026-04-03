using NoviVovi.Domain.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class CharacterStateMapper
{
    public CharacterStateDbO ToDbO(CharacterState character, Guid characterId)
    {
        var res = new CharacterStateDbO
        {
            Id = character.Id,
            CharacterId = characterId,
            Description = character.Description,
            ImageId = character.Image.Id,
            StateName = character.Name
        };
        return res;
    }

    public List<CharacterStateDbO> ToDbO(IEnumerable<CharacterState> character, Guid characterId)
    {
        return character.Select(characterState => ToDbO(characterState, characterId)).ToList();
    }

    // public CharacterState ToCharacterState(CharacterStateDbO dbo)
    // {
    //     var imgMapper = new ImageMapper();
    //     //загружаем картинку тут например
    //     var res = new CharacterState(dbo.Id,  маммер(картинкаДБО(dbo.ImageId)), ...);
    // }
}