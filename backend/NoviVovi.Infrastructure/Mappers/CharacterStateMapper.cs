using NoviVovi.Domain.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class CharacterStateMapper(ImageMapper imageMapper, TransformMapper transformMapper)
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

    public CharacterState ToDomain(CharacterStateDbO dbo)
    {
        //todo! заполнить все nullable поля
        return new CharacterState(
            dbo.Id,
            dbo.StateName,
            imageMapper.ToDomain(dbo.Image),
            transformMapper.ToDomain(dbo.Transform) ,
            dbo.Description);
    }
}