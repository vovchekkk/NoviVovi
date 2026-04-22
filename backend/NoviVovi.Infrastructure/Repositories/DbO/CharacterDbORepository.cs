using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

namespace NoviVovi.Infrastructure.Repositories.DbO;

public class CharacterDbORepository(
    DatabaseOptions options,
    IImageDbORepository imageDbORepository
) : BaseRepository(options), ICharacterDbORepository
{
    public async Task<IEnumerable<CharacterDbO>> GetByNovelIdsAsync(IEnumerable<Guid> novelIds)
    {
        if (!novelIds?.Any() ?? true) return Enumerable.Empty<CharacterDbO>();
        const string sql = @"SELECT 
                id AS Id,
                novel_id AS NovelId,
                name AS Name,
                name_color AS NameColor, 
                description AS Description
                             FROM ""Characters"" WHERE novel_id = ANY(@NovelIds) ORDER BY novel_id, name";
        return await QueryAsync<CharacterDbO>(sql, new { NovelIds = novelIds.ToArray() });
    }

    public async Task<IEnumerable<CharacterDbO>> GetFullByNovelIdsAsync(IEnumerable<Guid> novelIds)
    {
        if (!novelIds?.Any() ?? true) return Enumerable.Empty<CharacterDbO>();
        
        var characters = (await GetByNovelIdsAsync(novelIds)).ToList();
        
        foreach (var character in characters)
        {
            var states = await GetStatesByCharacterIdAsync(character.Id);
            foreach (var state in states)
            {
                state.Image = await imageDbORepository.GetImageByIdAsync(state.ImageId);
                if (state.TransformId.HasValue)
                    state.Transform = await imageDbORepository.GetTransformByIdAsync(state.TransformId.Value);
            }
        
            character.States = states.ToList();
        }
        
        return characters;
    }

    public async Task<CharacterStateDbO?> GetFullCharacterStateByIdAsync(Guid stateId)
    {
        const string sql = @"SELECT 
            id AS Id,
            character_id AS CharacterId, 
            image_id AS ImageId,
            state_name AS StateName,
            description AS Description,
            transform_id AS TransformId
                             FROM ""CharacterStates"" WHERE id = @StateId";
        
        var state = await QueryFirstOrDefaultAsync<CharacterStateDbO>(sql, new { StateId = stateId });
        if (state == null) return null;
        
        state.Image = await imageDbORepository.GetImageByIdAsync(state.ImageId);
        if (state.TransformId.HasValue)
            state.Transform = await imageDbORepository.GetTransformByIdAsync(state.TransformId.Value);
        
        return state;
    }

    public async Task<IEnumerable<CharacterDbO?>> GetFullByNovelIdAsync(Guid id)
    {
        return await GetFullByNovelIdsAsync([id]);
    }

    public async Task<CharacterDbO?> GetFullCharacterByIdAsync(Guid id)
    {
        const string sql = @"SELECT 
                id AS Id,
                novel_id AS NovelId,
                name AS Name,
                name_color AS NameColor, 
                description AS Description
                             FROM ""Characters"" WHERE id = @Id";
        var character = await QueryFirstOrDefaultAsync<CharacterDbO>(sql, new { Id = id });
        if (character != null)
        {
            var states = await GetStatesByCharacterIdAsync(character.Id);
            foreach (var state in states)
            {
                state.Image = await imageDbORepository.GetImageByIdAsync(state.ImageId);

                if (state.TransformId.HasValue)
                    state.Transform = await imageDbORepository.GetTransformByIdAsync(state.TransformId.Value);
            }

            character.States = states.ToList();
        }
        return character;
    }

    public async Task<Guid> AddAsync(CharacterDbO character)
    {
        const string sql = @"
            INSERT INTO ""Characters"" (id, novel_id, name, name_color, description)
            VALUES (@Id, @NovelId, @Name, @NameColor, @Description)";

        await ExecuteAsync(sql, character);
        return character.Id;
    }

    public async Task UpdateAsync(CharacterDbO character)
    {
        const string sql = @"
            UPDATE ""Characters"" SET
                novel_id = @NovelId,
                name = @Name,
                name_color = @NameColor,
                description = @Description
            WHERE id = @Id";

        await ExecuteAsync(sql, character);
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM \"Characters\" WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
    }

    public async Task<Guid> AddStepCharacterAsync(StepCharacterDbO stepCharacter)  //TODO: AddOrUpdateFull
    {
        const string sql = @"
            INSERT INTO ""StepCharacter"" (id, transform_id, character_state_id, step_id)
            VALUES (@Id, @TransformId, @CharacterStateId, @StepId)";
        
        if (stepCharacter.Transform != null)
            await imageDbORepository.AddTransformAsync(stepCharacter.Transform);
        if (stepCharacter.CharacterState != null)
            await AddOrUpdateStateAsync(stepCharacter.CharacterState);
        
        await ExecuteAsync(sql, stepCharacter);
        return stepCharacter.Id;
    }

    public async Task<bool> CheckIfExists(CharacterDbO character)
    {
        const string existsSql = @"SELECT 1 FROM ""Characters"" WHERE id = @Id LIMIT 1";
        var exists = await QueryFirstOrDefaultAsync<int?>(existsSql, new { character.Id });
        return exists.HasValue;
    }

    public async Task<Guid> AddOrUpdateFullAsync(CharacterDbO character)
    {
        if (character == null)
            throw new ArgumentNullException(nameof(character));

        var exists = await CheckIfExists(character);
        
        if (exists)
        {
            await UpdateAsync(character);  //тут не будет петли?
        }
        else
        {
            await AddAsync(character);
        }
        
        foreach (var state in character.States)
        {
            await AddOrUpdateStateAsync(state);
        }

        return character.Id;
    }

    public async Task<StepCharacterDbO?> GetFullStepCharacterByIdAsync(Guid id)
    {
        const string sql = @"
        SELECT id AS Id,
               transform_id AS TransformId,
               character_state_id AS CharacterStateId
        FROM ""StepCharacter""
        WHERE id = @Id";

        var character = await QueryFirstOrDefaultAsync<StepCharacterDbO>(sql, new { Id = id });
        if (character == null) return null;

        character.CharacterState =
            await GetFullCharacterStateByIdAsync(character.CharacterStateId);

        if (character.TransformId != null)
            character.Transform = await imageDbORepository.GetTransformByIdAsync(character.TransformId.Value);

        return character;
        //TODO: доставать вместе с Character из бд
    }

    public async Task DeleteStepCharacterAsync(Guid id)
    {
        const string sql = "DELETE FROM \"StepCharacter\" WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
    }

    public async Task DeleteStateAsync(Guid id)
    {
        const string sql = "DELETE FROM \"CharacterStates\" WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
    }

    public async Task UpdateStateAsync(CharacterStateDbO state)
    {
        const string sql = @"
        UPDATE ""CharacterStates"" SET
            character_id = @CharacterId,
            image_id = @ImageId,
            state_name = @StateName,
            description = @Description,
            transform_id = @TransformId
        WHERE id = @Id";
        
        await ExecuteAsync(sql, state);
    }

    private async Task<IEnumerable<CharacterStateDbO>> GetStatesByCharacterIdAsync(Guid characterId)
    {
        const string sql = @"SELECT id AS Id, character_id AS CharacterId, image_id AS ImageId,
                                    state_name AS StateName, description AS Description,
                                    transform_id AS TransformId
                             FROM ""CharacterStates"" WHERE character_id = @CharacterId ORDER BY state_name";
        var characters = await QueryAsync<CharacterStateDbO>(sql, new { CharacterId = characterId });
        foreach (var character in characters)
        {
            character.Image = await imageDbORepository.GetImageByIdAsync(character.ImageId);
            if (character.TransformId != null)
                character.Transform = await imageDbORepository.GetTransformByIdAsync(character.TransformId.Value);
        }
        
        return characters;
    }
    
    private async Task<bool> CheckStateIfExistsAsync(Guid id)
    {
        const string sql = @"SELECT 1 FROM ""CharacterStates"" WHERE id = @Id LIMIT 1";
        var result = await QueryFirstOrDefaultAsync<int?>(sql, new { Id = id });
        return result.HasValue;
    }

    private async Task<Guid> AddOrUpdateStateAsync(CharacterStateDbO state)
    {
        var exists = await CheckStateIfExistsAsync(state.Id);

        if (exists)
            await UpdateStateAsync(state);
        else
            await AddStateAsync(state);
        
        if (state.Transform != null)
            await imageDbORepository.AddOrUpdateTransformAsync(state.Transform);
        if (state.Image != null)
            await imageDbORepository.AddOrUpdateImageAsync(state.Image);
        return state.Id;
    }

    public async Task AddStateAsync(CharacterStateDbO state)
    {
        const string sql = @"
            INSERT INTO ""CharacterStates"" 
            (id, character_id, image_id, state_name, description, transform_id)
            VALUES 
            (@Id, @CharacterId, @ImageId, @StateName, @Description, @TransformId)";

        await ExecuteAsync(sql, state);
    }
}