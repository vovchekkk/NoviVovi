using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Scene;
using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class CharacterMapper(
    ImageMapper imageMapper,
    TransformMapper transformMapper
)
{
    public Character ToDomain(CharacterDbO dbo)
    {
        var res = new Character(dbo.Id, dbo.Name, dbo.NovelId, Color.FromHex(dbo.NameColor), dbo.Description);
        foreach (var state in dbo.States)
            res.AddCharacterState(ToDomain(state));
        return res;
    }

    public CharacterDbO ToDbO(Character character)
    {
        var res = new CharacterDbO
        {
            Id = character.Id,
            Name = character.Name,
            NameColor = character.NameColor.ToString().TrimStart('#'),
            NovelId = character.NovelId,
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
            StateName = character.Name,
            Image = imageMapper.ToDbO(character.Image),
        };
        return res;
    }

    public List<CharacterStateDbO> ToDbO(IEnumerable<CharacterState> character, Guid characterId)
    {
        return character.Select(characterState => ToDbO(characterState, characterId)).ToList();
    }

    public CharacterState ToDomain(CharacterStateDbO dbo)
    {
        if (dbo is { Transform: not null, Image: not null })
            return new CharacterState(
                dbo.Id,
                dbo.StateName,
                imageMapper.ToDomain(dbo.Image),
                transformMapper.ToDomain(dbo.Transform),
                dbo.Description);
        throw new ArgumentException("Invalid character state");
    }

    public StepCharacterDbO ToDbO(CharacterObject stepCharacterObject)
    {
        var res = new StepCharacterDbO
        {
            Id = stepCharacterObject.Id,
            CharacterStateId = stepCharacterObject.State.Id,
            CharacterState = ToDbO(stepCharacterObject.State, stepCharacterObject.Character.Id),
            TransformId = Guid.Empty,
            Transform = transformMapper.ToDbO(stepCharacterObject.Transform),
            Character = ToDbO(stepCharacterObject.Character)
        };
        return res;
    }

    public CharacterObject ToDomain(StepCharacterDbO stepCharacter)
    {
        if (stepCharacter is { CharacterState: not null, Transform: not null })
        {
            var res = new CharacterObject(stepCharacter.Id, ToDomain(stepCharacter.Character),
                ToDomain(stepCharacter.CharacterState),
                transformMapper.ToDomain(stepCharacter.Transform));
            return res;
        }
        
        throw new ArgumentException("Invalid step character");
    }
}