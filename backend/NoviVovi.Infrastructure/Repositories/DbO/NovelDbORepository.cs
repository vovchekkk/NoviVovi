using NoviVovi.Infrastructure.DatabaseObjects.Novels;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

namespace NoviVovi.Infrastructure.Repositories.DbO;

public class NovelDbORepository(
    string connectionString,
    ILabelDbORepository labelRepository,
    ICharacterDbORepository characterRepository)
    : BaseRepository(connectionString), INovelDbORepository
{

    public async Task<NovelDbO?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT 
                id AS Id,
                title AS Title,
                description AS Description,
                start_label_id AS StartLabelId,
                cover_image_id AS CoverImageId,
                is_public AS IsPublic,
                created_at AS CreatedAt,
                edited_at AS EditedAt
            FROM ""Novels""
            WHERE id = @Id";

        return await QueryFirstOrDefaultAsync<NovelDbO>(sql, new { Id = id });
    }

    public async Task<NovelDbO?> GetFullByIdAsync(Guid id)
    {
        var novel = await GetByIdAsync(id);
        if (novel == null) return null;
        if (novel.StartLabelId != null)
            novel.StartLabel = await labelRepository.GetFullByIdAsync(novel.StartLabelId.Value);
        novel.Labels = (await labelRepository.GetFullByNovelIdAsync(id))?.ToList() ?? [];
        novel.Characters = (await characterRepository.GetFullByNovelIdAsync(id))?.ToList() ?? [];
        return novel;
    }

    private async Task<IEnumerable<NovelDbO>> GetAllAsync(bool onlyPublic = true)
    {
        var sql = @"
            SELECT id, title, description, start_label_id, cover_image_id, 
                   is_public, created_at, edited_at
            FROM ""Novels""";
    
        if (onlyPublic)
            sql += " WHERE is_public = true";
    
        sql += " ORDER BY created_at DESC";
    
        return await QueryAsync<NovelDbO>(sql);
    }
    
    public async Task<IEnumerable<NovelDbO>> GetAllFullAsync(bool onlyPublic = true)
    {
        var novels = (await GetAllAsync(onlyPublic)).ToList();
        if (novels.Count == 0)
            return novels;
        
        var novelIds = novels.Select(n => n.Id).ToArray();
        
        var labelsTask = labelRepository.GetFullByNovelIdsAsync(novelIds);
        var charactersTask = characterRepository.GetFullByNovelIdsAsync(novelIds);

        await Task.WhenAll(labelsTask, charactersTask);

        var labelsByNovel = (await labelsTask).GroupBy(l => l.NovelId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var charsByNovel = (await charactersTask).GroupBy(c => c.NovelId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var novel in novels)
        {
            novel.Labels = labelsByNovel.TryGetValue(novel.Id, out var lbl) ? lbl : [];
            novel.Characters = charsByNovel.TryGetValue(novel.Id, out var ch) ? ch : [];
        }

        return novels;
    }
    
    
    public async Task<Guid> AddAsync(NovelDbO novel)
    {
        const string sql = @"
            INSERT INTO ""Novels"" (
                id, title, description, 
                start_label_id, cover_image_id, is_public, 
                created_at, edited_at
            ) VALUES (
                @Id, @Title, @Description, 
                @StartLabelId, @CoverImageId, @IsPublic, 
                @CreatedAt, @EditedAt
            )";

        await ExecuteAsync(sql, novel);
        return novel.Id;
    }

    public async Task UpdateAsync(NovelDbO dbo)
    {
        const string sql = @"
            UPDATE ""Novels"" SET title = @Title, description = @Description,
                start_label_id = @StartLabelId, cover_image_id = @CoverImageId,
                is_public = @IsPublic, edited_at = @EditedAt
            WHERE id = @Id";

        await ExecuteAsync(sql, dbo);
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM \"Novels\" WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
    }
}