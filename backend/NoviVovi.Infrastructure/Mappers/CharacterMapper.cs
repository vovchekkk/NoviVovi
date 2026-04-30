using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Scene;
using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class CharacterMapper(
    ImageMapper imageMapper,
    TransformMapper transformMapper,
    MappingContext ctx
)
{
    public Character ToDomain(CharacterDbO dbo)
    {
        if (ctx.Characters.TryGetValue(dbo.Id, out var cached))
            return cached;
        
        // Создаем Character и сразу добавляем в кэш ДО загрузки States
        var res = new Character(dbo.Id, dbo.Name, dbo.NovelId, Color.FromHex(dbo.NameColor), dbo.Description);
        ctx.Characters[dbo.Id] = res;
        
        // Теперь загружаем States - если будет рекурсия, вернется закэшированный Character
        foreach (var state in dbo.States)
            res.AddCharacterState(ToDomain(state));
            
        return res;
    }

    public CharacterDbO ToDbO(Character character)
    {
        if (ctx.CharacterDbOs.TryGetValue(character.Id, out var cached))
            return cached;
        
        var res = new CharacterDbO
        {
            Id = character.Id,
            Name = character.Name,
            NameColor = character.NameColor.ToString().TrimStart('#'),
            NovelId = character.NovelId,
            Description = character.Description,
            States = ToDbO(character.CharacterStates, character.Id)
        };
        
        ctx.CharacterDbOs[character.Id] = res;
        return res;
    }

    public CharacterStateDbO ToDbO(CharacterState character, Guid characterId)
    {
        if (ctx.CharacterStateDbOs.TryGetValue(character.Id, out var cached))
            return cached;
        
        var transformDbO = transformMapper.ToDbO(character.LocalTransform);
        
        var res = new CharacterStateDbO
        {
            Id = character.Id,
            CharacterId = characterId,
            Description = character.Description,
            ImageId = character.Image.Id,
            StateName = character.Name,
            Image = imageMapper.ToDbO(character.Image),
            Transform = transformDbO,
            TransformId = transformDbO.Id
        };
        
        ctx.CharacterStateDbOs[character.Id] = res;
        return res;
    }

    public List<CharacterStateDbO> ToDbO(IEnumerable<CharacterState> character, Guid characterId)
    {
        return character.Select(characterState => ToDbO(characterState, characterId)).ToList();
    }

    public CharacterState ToDomain(CharacterStateDbO dbo)
    {
        if (ctx.CharacterStates.TryGetValue(dbo.Id, out var cached))
            return cached;
        
        var res = new CharacterState(
                dbo.Id,
                dbo.StateName,
                imageMapper.ToDomain(dbo.Image),
                transformMapper.ToDomain(dbo.Transform),
                dbo.Description);
        
        ctx.CharacterStates[dbo.Id] = res;
        return res;
    }

    public StepCharacterDbO ToDbO(CharacterObject stepCharacterObject)
    {
        var transform = transformMapper.ToDbO(stepCharacterObject.Transform);
        var res = new StepCharacterDbO
        {
            Id = stepCharacterObject.Id,
            CharacterStateId = stepCharacterObject.State.Id,
            CharacterState = ToDbO(stepCharacterObject.State, stepCharacterObject.Character.Id),
            TransformId = transform.Id,
            Transform = transform,
            Character = ToDbO(stepCharacterObject.Character)
        };
        return res;
    }

    public CharacterObject ToDomain(StepCharacterDbO stepCharacter)
    {
        if (stepCharacter is { CharacterState: not null, Transform: not null })
        {
            var res = new CharacterObject(
                stepCharacter.Id,
                ToDomain(stepCharacter.Character), // Теперь загружаем полный Character со States
                ToDomain(stepCharacter.CharacterState),
                transformMapper.ToDomain(stepCharacter.Transform)
            );
            return res;
        }
        
        throw new ArgumentException("Invalid step character");
    }
}