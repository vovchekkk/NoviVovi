using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

namespace NoviVovi.Infrastructure.Repositories.New;

public class LabelDbORepository(string connectionString, IStepDbORepository stepDbORepository)
    : BaseRepository(connectionString), ILabelDbORepository
{
    private async Task<IEnumerable<LabelDbO>> GetByNovelIdsAsync(IEnumerable<Guid> novelIds)
    {
        if (!novelIds?.Any() ?? true)
            return Enumerable.Empty<LabelDbO>();

        const string sql = @"
            SELECT 
                id AS Id,
                novel_id AS NovelId,
                label_name AS LabelName
            FROM ""Labels""
            WHERE novel_id = ANY(@NovelIds)
            ORDER BY novel_id, label_name";

        return await QueryAsync<LabelDbO>(sql, new { NovelIds = novelIds.ToArray() });
    }
    
    private async Task<LabelDbO?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT 
                id AS Id,
                novel_id AS NovelId,
                label_name AS LabelName
            FROM ""Labels""
            WHERE label_id = @Id";
        return await QueryFirstOrDefaultAsync<LabelDbO>(sql, new { Id = id });
    }
    
    private async Task<IEnumerable<LabelDbO>> GetByNovelIdAsync(Guid novelId)
    {
        const string sql = @"
            SELECT 
                id AS Id,
                novel_id AS NovelId,
                label_name AS LabelName
            FROM ""Labels""
            WHERE novel_id = @NovelId
            ORDER BY label_name";

        return await QueryAsync<LabelDbO>(sql, new { NovelId = novelId });
    }

    public async Task<IEnumerable<LabelDbO?>> GetFullByNovelIdAsync(Guid novelId)
    {
        return await GetByNovelIdsAsync([novelId]);
    }
    
    public async Task<IEnumerable<LabelDbO>> GetFullByNovelIdsAsync(IEnumerable<Guid> novelIds)
    {
        if (!novelIds?.Any() ?? true)
            return [];

        var labels = (await GetByNovelIdsAsync(novelIds)).ToList();

        if (!labels.Any())
            return labels;

        var labelIds = labels.Select(l => l.Id).ToArray();
        var allFullSteps = await stepDbORepository.GetFullByLabelIdsAsync(labelIds);
        var stepsByLabel = allFullSteps.GroupBy(s => s.LabelId)
            .ToDictionary(g => g.Key, g => g.ToList());
        
        foreach (var label in labels)
        {
            label.Steps = stepsByLabel.TryGetValue(label.Id, out var steps) 
                ? steps 
                : [];
        }

        return labels;
    }

    public async Task<LabelDbO?> GetFullByIdAsync(Guid id)
    {
        var label = await GetByIdAsync(id);
        var steps = await stepDbORepository.GetOrderedByLabelIdAsync(label.Id);
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

    public async Task UpdateAsync(LabelDbO label)
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

    public async Task AddFullAsync(LabelDbO dbo) //TODO: AddOrUpdate
    {
        foreach (var step in dbo.Steps)
        {
            await stepDbORepository.AddFullAsync(step);
        }
        await AddAsync(dbo);
    }
}