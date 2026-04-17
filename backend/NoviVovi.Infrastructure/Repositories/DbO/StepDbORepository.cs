using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

namespace NoviVovi.Infrastructure.Repositories.DbO;

public class StepDbORepository(
    DatabaseOptions options,
    IMenuDbORepository menuRepo,
    ICharacterDbORepository characterRepository,
    ILabelDbORepository labelRepo,
    IImageDbORepository imageRepository
) : BaseRepository(options), IStepDbORepository
{
    public async Task<IEnumerable<StepDbO>> GetOrderedByLabelIdAsync(Guid labelId, LoadContext ctx)
    {
        const string sql = @"SELECT
            id AS Id,
            label_id AS LabelId,
            replica_id AS ReplicaId,
            menu_id AS MenuId,
            bg_id AS BackgroundId,
            character_id AS CharacterId,
            next_label_id AS NextLabelId,
            step_order AS StepOrder,
            step_type AS StepType
        FROM ""Steps""
        WHERE label_id = @LabelId
        ORDER BY step_order";

        var steps = (await QueryAsync<StepDbO>(sql, new { LabelId = labelId })).ToList();

        var result = new List<StepDbO>();

        foreach (var step in steps)
        {
            if (ctx.Steps.TryGetValue(step.Id, out var cached))
            {
                result.Add(cached);
                continue;
            }

            ctx.Steps[step.Id] = step;

            // MENU
            if (step.MenuId.HasValue)
                step.Menu = await menuRepo.GetFullByIdAsync(step.MenuId.Value, ctx);

            // NEXT LABEL (ВОТ ТУТ БЫЛ ЦИКЛ)
            if (step.NextLabelId.HasValue)
                step.NextLabel = await labelRepo.GetFullByIdAsync(step.NextLabelId.Value, ctx);

            result.Add(step);
        }

        return result;
    }

    public async Task<IEnumerable<StepDbO>> GetByLabelIdsAsync(IEnumerable<Guid> labelIds)
    {
        if (!labelIds?.Any() ?? true) return Enumerable.Empty<StepDbO>();
        const string sql = @"SELECT
                id AS Id,
                label_id AS LabelId,
                replica_id AS ReplicaId,
                menu_id AS MenuId,
                bg_id AS BgId,
                character_id AS CharacterId,
                next_label_id AS NextLabelId,
                step_order AS StepOrder,
                step_type AS StepType
                    FROM ""Steps"" WHERE label_id = ANY(@LabelIds) ORDER BY label_id, step_order";
        return await QueryAsync<StepDbO>(sql, new { LabelIds = labelIds.ToArray() });
    }

    public async Task<IEnumerable<StepDbO>> GetFullByLabelIdsAsync(IEnumerable<Guid> labelIds )
    {
        if (!labelIds?.Any() ?? true) return Enumerable.Empty<StepDbO>();

        var steps = (await GetByLabelIdsAsync(labelIds)).ToList();
        var fullSteps = new List<StepDbO>();

        foreach (var step in steps)
        {
            var ctx = new LoadContext();
            var full = await GetFullByIdAsync(step.Id, ctx);
            if (full != null) fullSteps.Add(full);
        }

        return fullSteps;
    }

    public async Task<StepDbO?> GetFullByIdAsync(Guid stepId, LoadContext ctx)
    {
        if (ctx.Steps.TryGetValue(stepId, out var cached))
            return cached;

        const string sql = @"
        SELECT id AS Id,
               label_id AS LabelId,
               replica_id AS ReplicaId,
               character_id AS CharacterId,
               menu_id AS MenuId,
               bg_id AS BackgroundId,
               next_label_id AS NextLabelId,
               step_order AS StepOrder,
               step_type AS StepType
        FROM ""Steps""
        WHERE id = @StepId";

        var step = await QueryFirstOrDefaultAsync<StepDbO>(sql, new { StepId = stepId });
        if (step == null) return null;

        ctx.Steps[stepId] = step;

        if (step.MenuId.HasValue)
            step.Menu = await menuRepo.GetFullByIdAsync(step.MenuId.Value, ctx);

        if (step.NextLabelId.HasValue)
            step.NextLabel = await labelRepo.GetFullByIdAsync(step.NextLabelId.Value, ctx);

        if (step.CharacterId.HasValue)
            step.Character = await characterRepository.GetFullStepCharacterByIdAsync(step.CharacterId.Value);

        if (step.BackgroundId.HasValue)
            step.Background = await imageRepository.GetFullBackgroundByIdAsync(step.BackgroundId.Value);

        if (step.ReplicaId.HasValue)
            step.Replica = await GetReplicaByIdAsync(step.ReplicaId.Value);

        return step;
    }

    private async Task<Guid> AddAsync(StepDbO step)
    {
        const string sql = @"
            INSERT INTO ""Steps"" (
                id, label_id, replica_id, menu_id, bg_id,
                next_label_id, step_order, step_type, character_id
            ) VALUES (
                @Id, @LabelId, @ReplicaId, @MenuId, @BgId,
                @NextLabelId, @StepOrder, @StepType, @CharacterId
            )";
        
        await ExecuteAsync(sql, step);
        return step.Id;
    }

    public async Task UpdateAsync(StepDbO step)
    {
        const string sql = @"
            UPDATE ""Steps"" SET
                label_id = @LabelId,
                replica_id = @ReplicaId,
                menu_id = @MenuId,
                bg_id = @BgId,
                character_id AS @CharacterId,
                next_label_id = @NextLabelId,
                step_order = @StepOrder,
                step_type = @StepType
            WHERE id = @Id";
        
        await ExecuteAsync(sql, step);
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM \"Steps\" WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
    }
    
    private async Task<bool> CheckIfExistsAsync(Guid id)
    {
        const string sql = @"SELECT 1 FROM ""Steps"" WHERE id = @Id LIMIT 1";
        var result = await QueryFirstOrDefaultAsync<int?>(sql, new { Id = id });
        return result.HasValue;
    }
    
    public async Task AddOrUpdateFullAsync(StepDbO step, LoadContext ctx)
    {
        if (ctx.Steps.ContainsKey(step.Id))
            return;

        ctx.Steps[step.Id] = step;
        
        var exists = await CheckIfExistsAsync(step.Id);

        if (exists)
            await UpdateAsync(step);
        else
            await AddAsync(step);

        if (step.Replica != null)
        {
            await CreateReplicaAsync(step.Replica);
        }

        if (step.Menu != null)
        {
            await menuRepo.AddOrUpdateFullAsync(step.Menu, ctx);
        }

        if (step.Background != null)
        {
            await imageRepository.AddBgAsync(step.Background);
        }

        if (step.Character != null)
        {
            await characterRepository.AddStepCharacterAsync(step.Character);
        }

        if (step.NextLabel != null)
        {
            await labelRepo.AddOrUpdateFullAsync(step.NextLabel, ctx);
        }
    }
    
    private async Task<ReplicaDbO?> GetReplicaByIdAsync(Guid id)
    {
        const string sql = @"SELECT id AS Id, speaker_id AS SpeakerId, text AS Text
                             FROM ""Replicas"" WHERE id = @Id";
        var replica = await QueryFirstOrDefaultAsync<ReplicaDbO>(sql, new { Id = id });
        if (replica == null) return null;
        if (replica.SpeakerId != null)
            replica.Speaker = await characterRepository.GetFullCharacterByIdAsync(replica.SpeakerId.Value);
        return replica;
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