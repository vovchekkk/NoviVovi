using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

namespace NoviVovi.Infrastructure.Repositories.DbO;

public class CharacterDbORepository : BaseRepository, ICharacterDbORepository
{
    private readonly IImageDbORepository imageDbORepository;

    public CharacterDbORepository(
        IUnitOfWork unitOfWork,
        IImageDbORepository imageDbORepository
    ) : base(unitOfWork)
    {
        this.imageDbORepository = imageDbORepository;
    }
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
                name = @Name,
                name_color = @NameColor,
                description = @Description
            WHERE id = @Id";

        await ExecuteAsync(sql, character);
    }

    public async Task DeleteAsync(Guid id)
    {
        // 1. Get all CharacterStates for this Character
        const string getStatesSql = "SELECT \"id\" FROM \"CharacterStates\" WHERE \"character_id\" = @CharacterId";
        var stateIds = (await QueryAsync<Guid>(getStatesSql, new { CharacterId = id })).ToList();
        
        // 2. Delete all CharacterStates (this will cascade delete StepCharacters and Transforms)
        foreach (var stateId in stateIds)
        {
            await DeleteStateAsync(stateId);
        }
        
        // 3. Delete all HideCharacterSteps that reference this Character
        const string deleteHideStepsSql = "DELETE FROM \"Steps\" WHERE \"hide_character_id\" = @CharacterId";
        await ExecuteAsync(deleteHideStepsSql, new { CharacterId = id });
        
        // 4. Delete all Replicas where this Character is the Speaker
        const string deleteReplicasSql = "DELETE FROM \"Replicas\" WHERE \"speaker_id\" = @CharacterId";
        await ExecuteAsync(deleteReplicasSql, new { CharacterId = id });
        
        // 5. Delete the Character itself
        const string sql = "DELETE FROM \"Characters\" WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
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
            await UpdateAsync(character);
            
            // Get existing state IDs from database
            const string getExistingStatesSql = @"SELECT ""id"" FROM ""CharacterStates"" WHERE ""character_id"" = @CharacterId";
            var existingStateIds = (await QueryAsync<Guid>(getExistingStatesSql, new { CharacterId = character.Id })).ToList();
            
            // Get new state IDs from domain model
            var newStateIds = character.States.Select(s => s.Id).ToList();
            
            // Find states that need to be deleted (exist in DB but not in domain model)
            var stateIdsToDelete = existingStateIds.Except(newStateIds).ToList();
            
            // Delete only the states that are no longer in the domain model
            if (stateIdsToDelete.Any())
            {
                foreach (var stateId in stateIdsToDelete)
                {
                    await DeleteStateAsync(stateId);
                }
            }
        }
        else
        {
            await AddAsync(character);
        }
        
        // Add or update all states from the domain model
        foreach (var state in character.States)
        {
            await AddOrUpdateStateAsync(state);
        }

        return character.Id;
    }

    public async Task<StepCharacterDbO?> GetCharacterObjectByCharacterIdAsync(Guid stepCharacterId)
    {
        const string sql = @"
        SELECT id AS Id,
               transform_id AS TransformId,
               character_state_id AS CharacterStateId
        FROM ""StepCharacter""
        WHERE id = @StepCharacterId";

        var character = await QueryFirstOrDefaultAsync<StepCharacterDbO>(sql, new { StepCharacterId = stepCharacterId });
        if (character == null) return null;

        character.CharacterState =
            await GetFullCharacterStateByIdAsync(character.CharacterStateId);

        if (character.TransformId != null)
            character.Transform = await imageDbORepository.GetTransformByIdAsync(character.TransformId.Value);
        
        if(character.CharacterState != null) 
            character.Character = await GetFullCharacterByIdAsync(character.CharacterState.CharacterId);
        
        return character;
    }

    public async Task DeleteStepCharacterAsync(Guid id)
    {
        const string sql = "DELETE FROM \"StepCharacter\" WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
    }

    public async Task DeleteStateAsync(Guid id)
    {
        // 1. Get all StepCharacters that use this CharacterState
        const string getStepCharactersSql = @"
            SELECT ""id"", ""transform_id"" 
            FROM ""StepCharacter"" 
            WHERE ""character_state_id"" = @StateId";
        var stepCharacters = await QueryAsync<(Guid Id, Guid? TransformId)>(getStepCharactersSql, new { StateId = id });
        
        // 2. Delete all Steps that reference these StepCharacters (BEFORE deleting StepCharacters)
        foreach (var (stepCharId, _) in stepCharacters)
        {
            const string deleteStepSql = "DELETE FROM \"Steps\" WHERE \"character_id\" = @CharacterId";
            await ExecuteAsync(deleteStepSql, new { CharacterId = stepCharId });
        }
        
        // 3. Delete all StepCharacters and their Transforms
        foreach (var (stepCharId, stepTransformId) in stepCharacters)
        {
            const string deleteStepCharSql = "DELETE FROM \"StepCharacter\" WHERE \"id\" = @Id";
            await ExecuteAsync(deleteStepCharSql, new { Id = stepCharId });
            
            // Delete Transform manually (double CASCADE doesn't work reliably)
            if (stepTransformId.HasValue)
            {
                await imageDbORepository.DeleteTransformById(stepTransformId.Value);
            }
        }
        
        // 4. Get transform_id of the CharacterState itself
        const string getTransformSql = "SELECT transform_id FROM \"CharacterStates\" WHERE id = @Id";
        var transformId = await QueryFirstOrDefaultAsync<Guid?>(getTransformSql, new { Id = id });
        
        // 5. Delete the CharacterState
        const string sql = "DELETE FROM \"CharacterStates\" WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
        
        // 6. Delete the CharacterState's Transform manually (double CASCADE doesn't work reliably)
        if (transformId.HasValue)
        {
            await imageDbORepository.DeleteTransformById(transformId.Value);
        }
    }

    public async Task UpdateStateAsync(CharacterStateDbO state)
    {
        const string sql = @"
        UPDATE ""CharacterStates"" SET
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

    public async Task<Guid> AddOrUpdateStateAsync(CharacterStateDbO state)
    {
        var exists = await CheckStateIfExistsAsync(state.Id);
        
        if (state.Transform != null)
            await imageDbORepository.AddOrUpdateTransformAsync(state.Transform);
        if (state.Image != null)
            await imageDbORepository.AddOrUpdateImageAsync(state.Image);
        
        if (exists)
            await UpdateStateAsync(state);
        else
            await AddStateAsync(state);
        
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
    
    public async Task<Guid> AddOrUpdateStepCharacterAsync(StepCharacterDbO stepCharacter)
    {
        // Сохраняем связанные объекты
        if (stepCharacter.Transform != null)
            await imageDbORepository.AddOrUpdateTransformAsync(stepCharacter.Transform);
    
        if (stepCharacter.CharacterState != null)
            await AddOrUpdateStateAsync(stepCharacter.CharacterState);
        
        var exists = await CheckStepCharacterExistsAsync(stepCharacter.Id);
    
        if (exists)
            await UpdateStepCharacterAsync(stepCharacter);
        else
            await AddStepCharacterInternalAsync(stepCharacter);
    
        return stepCharacter.Id;
    }

    private async Task<bool> CheckStepCharacterExistsAsync(Guid id)
    {
        const string sql = "SELECT 1 FROM \"StepCharacter\" WHERE id = @Id LIMIT 1";
        var result = await QueryFirstOrDefaultAsync<int?>(sql, new { Id = id });
        return result.HasValue;
    }

    private async Task UpdateStepCharacterAsync(StepCharacterDbO stepCharacter)
    {
        const string sql = @"
        UPDATE ""StepCharacter"" 
        SET transform_id = @TransformId,
            character_state_id = @CharacterStateId
        WHERE id = @Id";
    
        await ExecuteAsync(sql, stepCharacter);
    }

    private async Task AddStepCharacterInternalAsync(StepCharacterDbO stepCharacter)
    {
        const string sql = @"
        INSERT INTO ""StepCharacter"" (id, transform_id, character_state_id)
        VALUES (@Id, @TransformId, @CharacterStateId)";
    
        await ExecuteAsync(sql, stepCharacter);
    }
}