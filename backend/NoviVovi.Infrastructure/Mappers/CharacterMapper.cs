using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Scene;
using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class CharacterMapper(ImageMapper imageMapper, TransformMapper transformMapper)
{
    public Character ToCharacter(CharacterDbO dbo)
    {
        var res = new Character(dbo.Id, dbo.Name, dbo.Description);
        foreach (var state in dbo.States)
            res.AddState(ToDomain(state));
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
            States = ToDbO(character.CharacterStates, character.Id)
        };
        return res;
    }
    
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

    public StepCharacterDbO ToDbO(CharacterObject stepCharacterObject)
    {
        throw new NotImplementedException();
    }

    public CharacterObject ToDomain(StepDbO step)
    {
        throw new NotImplementedException();
    }
}