using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

namespace NoviVovi.Infrastructure.Repositories.New;

public class LabelDbORepository : BaseRepository, ILabelDbORepository
{
    private readonly Lazy<IStepDbORepository> stepRepo;

    public LabelDbORepository(
        IUnitOfWork unitOfWork,
        Lazy<IStepDbORepository> stepRepo
    ) : base(unitOfWork)
    {
        this.stepRepo = stepRepo;
    }

    private async Task<LabelDbO?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT
                id AS Id,
                novel_id AS NovelId,
                label_name AS LabelName
            FROM ""Labels""
            WHERE id = @Id";
        return await QueryFirstOrDefaultAsync<LabelDbO>(sql, new { Id = id });
    }

    // private async Task<IEnumerable<LabelDbO>> GetByNovelIdAsync(Guid novelId)
    // {
    //     const string sql = @"
    //         SELECT
    //             id AS Id,
    //             novel_id AS NovelId,
    //             label_name AS LabelName
    //         FROM ""Labels""
    //         WHERE novel_id = @NovelId
    //         ORDER BY label_name";
    //
    //     return await QueryAsync<LabelDbO>(sql, new { NovelId = novelId });
    // }
    
    

    public async Task<IEnumerable<LabelDbO?>> GetFullByNovelIdAsync(Guid novelId)
    {
        const string sql = @"
            SELECT
                id AS Id,
                novel_id AS NovelId,
                label_name AS LabelName
            FROM ""Labels""
            WHERE novel_id = @NovelId
            ORDER BY novel_id, label_name";

        var labels = await QueryAsync<LabelDbO>(sql, new { NovelId = novelId }); 
        var ctx = new LoadContext();
        
        return await GetFull(labels, ctx);
    }

    public async Task<IEnumerable<LabelDbO>> GetFullByNovelIdsAsync(IEnumerable<Guid> novelIds)
    {
        const string sql = @"
            SELECT
                id AS Id,
                novel_id AS NovelId,
                label_name AS LabelName
            FROM ""Labels""
            WHERE novel_id = ANY(@NovelIds)
            ORDER BY novel_id, label_name";

        var labels = await QueryAsync<LabelDbO>(sql, new { NovelIds = novelIds }); 
        var ctx = new LoadContext();
        
        return await GetFull(labels, ctx);
    }

    public async Task<IEnumerable<LabelDbO>> GetFullByIdsAsync(IEnumerable<Guid> ids)
    {
        if (!ids?.Any() ?? true)
            return Enumerable.Empty<LabelDbO>();
        
        const string sql = @"
            SELECT
                id AS Id,
                novel_id AS NovelId,
                label_name AS LabelName
            FROM ""Labels""
            WHERE id = ANY(@Ids)
            ORDER BY novel_id, label_name";

        var labels = await QueryAsync<LabelDbO>(sql, new { Ids = ids.ToArray() });
        var ctx = new LoadContext();
        
        return await GetFull(labels, ctx);
    }

    private async Task<IEnumerable<LabelDbO>> GetFull(IEnumerable<LabelDbO> labels, LoadContext ctx)
    {
        foreach (var label in labels)
        {
            if (ctx.Labels.ContainsKey(label.Id))
                continue;

            ctx.Labels[label.Id] = label;

            var steps = await stepRepo.Value.GetOrderedByLabelIdAsync(label.Id, ctx);
            label.Steps = steps.ToList();
        }

        return labels;
    }
    
    public async Task<LabelDbO?> GetFullByIdAsync(Guid id, LoadContext ctx)
    {
        if (ctx.Labels.TryGetValue(id, out var cached))
            return cached;

        const string sql = @"
        SELECT id AS Id, novel_id AS NovelId, label_name AS LabelName
        FROM ""Labels""
        WHERE id = @Id";

        var label = await QueryFirstOrDefaultAsync<LabelDbO>(sql, new { Id = id });
        if (label == null) return null;

        ctx.Labels[id] = label;

        var steps = await stepRepo.Value.GetOrderedByLabelIdAsync(id, ctx);
        label.Steps = steps.ToList();

        return label;
    }

    public async Task<Guid> AddAsync(LabelDbO label)
    {
        const string sql = @"
            INSERT INTO ""Labels"" (id, novel_id, label_name)
            VALUES (@Id, @NovelId, @LabelName)";

        await ExecuteAsync(sql, label);
        return label.Id;
    }

    private async Task UpdateAsync(LabelDbO label)
    {
        const string sql = @"
            UPDATE ""Labels"" SET
                novel_id = @NovelId,
                label_name = @LabelName
            WHERE id = @Id";

        await ExecuteAsync(sql, label);
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM \"Labels\" WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
    }

    private async Task<bool> CheckIfExistsAsync(Guid id)
    {
        const string sql = @"SELECT 1 FROM ""Labels"" WHERE id = @Id LIMIT 1";
        var result = await QueryFirstOrDefaultAsync<int?>(sql, new { Id = id });
        return result.HasValue;
    }
    
    public async Task<Guid> AddOrUpdateFullAsync(LabelDbO label, LoadContext? ctx = null)
    {
        ctx ??= new LoadContext();
        
        if (ctx.Labels.ContainsKey(label.Id))
            return label.Id;

        ctx.Labels[label.Id] = label;

        var exists = await CheckIfExistsAsync(label.Id);

        if (exists)
            await UpdateAsync(label);
        else
            await AddAsync(label);
        
        var existingStepIds = await stepRepo.Value.GetStepIdsByLabelIdAsync(label.Id);
        var newStepIds = label.Steps?.Select(s => s.Id).ToHashSet() ?? new HashSet<Guid>();
        var stepIdsToDelete = existingStepIds.Except(newStepIds).ToList();
        
        foreach (var stepId in stepIdsToDelete)
        {
            await stepRepo.Value.DeleteStepAsync(stepId);
        }
        
        if (label.Steps != null && label.Steps.Any())
        {
            foreach (var step in label.Steps)
            {
                await stepRepo.Value.AddOrUpdateFullAsync(step, ctx);
            }
        }

        return label.Id;
    }
}