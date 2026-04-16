using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

namespace NoviVovi.Infrastructure.Repositories.DbO;

public class StepDbORepository(
    DatabaseOptions options/*,
    IMenuDbORepository menuRepository,
    ICharacterDbORepository characterRepository,
    ILabelDbORepository labelRepository,
    IImageDbORepository imageRepository*/
) : BaseRepository(options), IStepDbORepository
{
    public async Task<IEnumerable<StepDbO>> GetOrderedByLabelIdAsync(Guid labelId)
    {
        throw new NotImplementedException();
        // const string sql = @"SELECT
        //         id AS Id,
        //         label_id AS LabelId,
        //         replica_id AS ReplicaId,
        //         menu_id AS MenuId,
        //         bg_id AS BgId,
        //         character_id AS CharacterId,
        //         next_label_id AS NextLabelId,
        //         step_order AS StepOrder, step_type AS StepType
        //              FROM ""Steps"" WHERE label_id = @LabelId ORDER BY step_order";
        // return await QueryAsync<StepDbO>(sql, new { LabelId = labelId });
    }

    public async Task<IEnumerable<StepDbO>> GetByLabelIdsAsync(IEnumerable<Guid> labelIds)
    {
        throw new NotImplementedException();
        // if (!labelIds?.Any() ?? true) return Enumerable.Empty<StepDbO>();
        // const string sql = @"SELECT
        //         id AS Id,
        //         label_id AS LabelId,
        //         replica_id AS ReplicaId,
        //         menu_id AS MenuId,
        //         bg_id AS BgId,
        //         character_id AS CharacterId,
        //         next_label_id AS NextLabelId,
        //         step_order AS StepOrder,
        //         step_type AS StepType
        //             FROM ""Steps"" WHERE label_id = ANY(@LabelIds) ORDER BY label_id, step_order";
        // return await QueryAsync<StepDbO>(sql, new { LabelIds = labelIds.ToArray() });
    }

    public async Task<IEnumerable<StepDbO>> GetFullByLabelIdsAsync(IEnumerable<Guid> labelIds)
    {
        if (!labelIds?.Any() ?? true) return Enumerable.Empty<StepDbO>();

        var steps = (await GetByLabelIdsAsync(labelIds)).ToList();
        var fullSteps = new List<StepDbO>();

        foreach (var step in steps)
        {
            var full = await GetFullByIdAsync(step.Id);
            if (full != null) fullSteps.Add(full);
        }

        return fullSteps;
    }

    public async Task<StepDbO?> GetFullByIdAsync(Guid stepId)
    {
        throw new NotImplementedException();
        // const string sql = @"SELECT
        //         id AS Id,
        //         label_id AS LabelId,
        //         replica_id AS ReplicaId,
        //         character_id AS CharacterId,
        //         menu_id AS MenuId,
        //         bg_id AS BgId,
        //         next_label_id AS NextLabelId,
        //         step_order AS StepOrder, step_type AS StepType
        //             FROM ""Steps"" WHERE id = @StepId";
        //
        // var step = await QueryFirstOrDefaultAsync<StepDbO>(sql, new { StepId = stepId });
        // if (step == null) return null;
        //
        // if (step.ReplicaId.HasValue)
        //     step.Replica = await GetReplicaByIdAsync(step.ReplicaId.Value);
        //
        // if (step.MenuId.HasValue)
        //     step.Menu = await menuRepository.GetFullByIdAsync(step.MenuId.Value);
        //
        // if (step.BackgroundId.HasValue)
        //     step.Background = await GetFullBackgroundByIdAsync(step.BackgroundId.Value);
        //
        // if (step.CharacterId.HasValue)
        //     step.Character = await GetFullStepCharactersByIdAsync(step.CharacterId.Value);
        //
        // if (step.NextLabelId.HasValue)
        //     step.NextLabel = await GetLabelByIdAsync(step.NextLabelId.Value);
        //
        // return step;
    }

    public async Task<Guid> AddAsync(StepDbO step)
    {
        throw new NotImplementedException();
        // const string sql = @"
        //     INSERT INTO ""Steps"" (
        //         id, label_id, replica_id, menu_id, bg_id,
        //         next_label_id, step_order, step_type, character_id
        //     ) VALUES (
        //         @Id, @LabelId, @ReplicaId, @MenuId, @BgId,
        //         @NextLabelId, @StepOrder, @StepType, @CharacterId
        //     )";
        //
        // await ExecuteAsync(sql, step);
        // return step.Id;
    }

    public async Task UpdateAsync(StepDbO step)
    {
        throw new NotImplementedException();
        // const string sql = @"
        //     UPDATE ""Steps"" SET
        //         label_id = @LabelId,
        //         replica_id = @ReplicaId,
        //         menu_id = @MenuId,
        //         bg_id = @BgId,
        //         character_id AS @CharacterId,
        //         next_label_id = @NextLabelId,
        //         step_order = @StepOrder,
        //         step_type = @StepType
        //     WHERE id = @Id";
        //
        // await ExecuteAsync(sql, step);
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM \"Steps\" WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
    }

    public async Task AddFullAsync(StepDbO step) //TODO: AddOrUpdate
    {
        throw new NotImplementedException();
        // if (step.Replica != null)
        //     await CreateReplicaAsync(step.Replica);
        // if (step.Menu != null)
        //     await menuRepository.AddFullAsync(step.Menu);
        // if (step.Background != null)
        //     await imageRepository.AddBgAsync(step.Background);
        // if (step.NextLabel != null)
        //     await labelRepository.AddFullAsync(step.NextLabel);
        // if (step.Character != null)
        //     await characterRepository.AddStepCharacterAsync(step.Character);
    }

    // ========================================

    private async Task<ReplicaDbO?> GetReplicaByIdAsync(Guid id)
    {
        throw new NotImplementedException();
        // const string sql = @"SELECT id AS Id, speaker_id AS SpeakerId, text AS Text
        //                      FROM ""Replicas"" WHERE id = @Id";
        // var replica = await QueryFirstOrDefaultAsync<ReplicaDbO>(sql, new { Id = id });
        // if (replica == null) return null;
        // if (replica.SpeakerId != null)
        //     replica.Speaker = await characterRepository.GetFullCharacterByIdAsync(replica.SpeakerId.Value);
        // return replica;
    }

    private async Task<LabelDbO?> GetLabelByIdAsync(Guid id)
    {
        throw new NotImplementedException();
        // return await labelRepository.GetFullByIdAsync(id);
    }

    private async Task<BackgroundDbO?> GetFullBackgroundByIdAsync(Guid bgId)
    {
        throw new NotImplementedException();
        // const string sql = @"SELECT id AS Id, img AS Img, transform_id AS TransformId
        //                      FROM ""Backgrounds"" WHERE id = @BgId";
        // var bg = await QueryFirstOrDefaultAsync<BackgroundDbO>(sql, new { BgId = bgId });
        // if (bg == null) return null;
        //
        // bg.Image = await imageRepository.GetImageByIdAsync(bg.Img);
        // if (bg.TransformId.HasValue)
        //     bg.Transform = await imageRepository.GetTransformByIdAsync(bg.TransformId.Value);
        //
        // return bg;
    }

    private async Task<StepCharacterDbO?> GetFullStepCharactersByIdAsync(Guid id)
    {
        throw new NotImplementedException();
        // const string sql = @"SELECT
        //         id AS Id,
        //         transform_id AS TransformId,
        //         character_state_id AS CharacterStateId
        //                      FROM ""StepCharacter"" WHERE id = @id";
        // var character = await QueryFirstOrDefaultAsync<StepCharacterDbO>(sql, new { Id = id });
        // if (character == null) return null;
        // character.CharacterState = await characterRepository.GetFullCharacterStateByIdAsync(character.CharacterStateId);
        // if (character.CharacterState != null)
        //     character.Character =
        //         await characterRepository.GetFullCharacterByIdAsync(character.CharacterState.CharacterId);
        // return character;
    }


    public async Task DeleteReplicaAsync(Guid id)
    {
        const string sql = "DELETE FROM \"Replicas\" WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
    }

    public async Task UpdateReplicaAsync(ReplicaDbO replica)
    {
        const string sql = @"
            UPDATE ""Replicas"" SET
                speaker_id = @SpeakerId,
                text = @Text
            WHERE id = @Id";

        await ExecuteAsync(sql, replica);
    }

    public async Task<Guid> CreateReplicaAsync(ReplicaDbO replica)
    {
        const string sql = @"
            INSERT INTO ""Replicas"" (id, speaker_id, text)
            VALUES (@Id, @SpeakerId, @Text)";

        await ExecuteAsync(sql, replica);
        return replica.Id;
    }
}