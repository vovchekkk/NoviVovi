using NoviVovi.Infrastructure.DatabaseObjects.Novels;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

namespace NoviVovi.Infrastructure.Repositories.DbO;

public class NovelDbORepository(
    DatabaseOptions options,
    ILabelDbORepository labelRepository,
    ICharacterDbORepository characterRepository
) : BaseRepository(options), INovelDbORepository
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

    public async Task<NovelDbO?> GetFullByIdAsync(Guid id, LoadContext ctx)
    {
        var novel = await GetByIdAsync(id);
        if (novel == null) return null;
        if (novel.StartLabelId != null)
            novel.StartLabel = await labelRepository.GetFullByIdAsync(novel.StartLabelId.Value, ctx);
        novel.Labels = (await labelRepository.GetFullByNovelIdAsync(id))?.ToList() ?? [];
        novel.Characters = (await characterRepository.GetFullByNovelIdAsync(id))?.ToList() ?? [];
        return novel;
    }

    private async Task<IEnumerable<NovelDbO>> GetAllAsync(bool onlyPublic = true)
    {
        var sql = @"
            SELECT
                id AS Id,
                title AS Title,
                description AS Description,
                start_label_id AS StartLabelId,
                cover_image_id AS CoverImageId,
                is_public AS IsPublic,
                created_at AS CreatedAt,
                edited_at AS EditedAt
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
            novel.StartLabel = novel.Labels.FirstOrDefault(l => l.Id == novel.StartLabelId);
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
    
    public async Task<bool> CheckIfExistsAsync(Guid id)
    {
        const string sql = @"SELECT 1 FROM ""Novels"" WHERE id = @Id LIMIT 1";
        var result = await QueryFirstOrDefaultAsync<int?>(sql, new { Id = id });
        return result.HasValue;
    }
    
    
    public async Task<Guid> AddOrUpdateFullAsync(NovelDbO novel)
    {
        var ctx = new LoadContext();
        

        var exists = await CheckIfExistsAsync(novel.Id);

        if (exists)
            await UpdateAsync(novel);
        else
            await AddAsync(novel);

        
        if (novel.StartLabel != null)
        {
            novel.StartLabel.NovelId = novel.Id;
            await labelRepository.AddOrUpdateFullAsync(novel.StartLabel, ctx);
        }
        
        var existingLabels = (await labelRepository.GetFullByNovelIdAsync(novel.Id)).ToList();
        var existingLabelIds = existingLabels.Select(l => l.Id).ToHashSet();
        var newLabelIds = novel.Labels.Select(l => l.Id).ToHashSet();
        var toDeleteLabels = existingLabelIds.Except(newLabelIds).ToList();
        foreach (var labelId in toDeleteLabels)
        {
            await labelRepository.DeleteAsync(labelId);
        }

        foreach (var label in novel.Labels)
        {
            label.NovelId = novel.Id;
            await labelRepository.AddOrUpdateFullAsync(label, ctx);
        }
        
        var existingCharacters = (await characterRepository.GetFullByNovelIdAsync(novel.Id)).ToList();
        var existingCharIds = existingCharacters.Select(c => c.Id).ToHashSet();
        var newCharIds = novel.Characters.Select(c => c.Id).ToHashSet();
        var toDeleteChars = existingCharIds.Except(newCharIds).ToList();
        foreach (var charId in toDeleteChars)
        {
            await characterRepository.DeleteAsync(charId);
        }

        foreach (var character in novel.Characters)
        {
            character.NovelId = novel.Id;
            await characterRepository.AddOrUpdateFullAsync(character);
        }

        return novel.Id;
    }
}