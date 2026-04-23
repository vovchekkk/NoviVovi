using NoviVovi.Infrastructure.DatabaseObjects.Choices;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

namespace NoviVovi.Infrastructure.Repositories.DbO;

public class MenuDbORepository(
    DatabaseOptions options,
    Lazy<ILabelDbORepository> labelRepo
) : BaseRepository(options), IMenuDbORepository
{
    
    public async Task<MenuDbO?> GetFullByIdAsync(Guid menuId, LoadContext ctx)
    {
        if (ctx.Menus.TryGetValue(menuId, out var cached))
            return cached;

        const string menuSql = @"SELECT id AS Id, name AS Name, text AS Text, description AS Description
                             FROM ""Menus"" WHERE id = @MenuId";

        var menu = await QueryFirstOrDefaultAsync<MenuDbO>(menuSql, new { MenuId = menuId });
        if (menu == null) return null;

        ctx.Menus[menuId] = menu;

        const string choicesSql = @"SELECT id AS Id, menu_id AS MenuId, next_label_id AS NextLabelId,
                                      name AS Name, text AS Text
                               FROM ""Choices"" WHERE menu_id = @MenuId ORDER BY name";

        var choices = (await QueryAsync<ChoiceDbO>(choicesSql, new { MenuId = menuId })).ToList();

        foreach (var choice in choices)
        {
            choice.NextLabel = await labelRepo.Value.GetFullByIdAsync(choice.NextLabelId, ctx);
        }

        menu.Choices = choices;
        return menu;
    }

    public async Task<Guid> AddAsync(MenuDbO menu)
    {
        const string sql = @"
            INSERT INTO ""Menus"" (id, name, text, description)
            VALUES (@Id, @Name, @Text, @Description)";

        await ExecuteAsync(sql, menu);
        return menu.Id;
    }

    public async Task<Guid> AddChoiceAsync(ChoiceDbO choice)
    {
        const string sql = @"
            INSERT INTO ""Choices"" (id, menu_id, next_label_id, name, text)
            VALUES (@Id, @MenuId, @NextLabelId, @Name, @Text)";

        await ExecuteAsync(sql, choice);
        return choice.Id;
    }

    public async Task DeleteChoiceAsync(Guid choiceId)
    {
        const string sql = "DELETE FROM \"Choices\" WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = choiceId });
    }

    private async Task UpdateChoiceAsync(ChoiceDbO choice)
    {
        const string sql = @"
            UPDATE ""Choices""
            SET next_label_id = @NextLabelId,
                name = @Name,
                text = @Text
            WHERE id = @Id";

        await ExecuteAsync(sql, choice);
    }

    public async Task UpdateAsync(MenuDbO menu)
    {
        const string sql = @"
            UPDATE ""Menus""
            SET name = @Name,
                text = @Text,
                description = @Description
            WHERE id = @Id";

        await ExecuteAsync(sql, menu);
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = @"DELETE FROM ""Menus"" WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
    }
    
    public async Task<bool> CheckIfExistsAsync(Guid id)
    {
        const string sql = @"SELECT 1 FROM ""Menus"" WHERE id = @Id LIMIT 1";
        var result = await QueryFirstOrDefaultAsync<int?>(sql, new { Id = id });
        return result.HasValue;
    }
    
    public async Task<bool> CheckChoiceExistsAsync(Guid id)
    {
        const string sql = @"SELECT 1 FROM ""Choices"" WHERE id = @Id LIMIT 1";
        var result = await QueryFirstOrDefaultAsync<int?>(sql, new { Id = id });
        return result.HasValue;
    }
    
    public async Task<Guid> AddOrUpdateFullAsync(MenuDbO menu, LoadContext? ctx = null)
    {
        ctx ??= new LoadContext();
        
        if (ctx.Menus.ContainsKey(menu.Id))
            return menu.Id;

        ctx.Menus[menu.Id] = menu;

        var exists = await CheckIfExistsAsync(menu.Id);

        if (exists)
            await UpdateAsync(menu);
        else
            await AddAsync(menu);
        
        var existingChoiceIds = await GetChoiceIdsByMenuIdAsync(menu.Id);
        var newChoiceIds = menu.Choices?.Select(c => c.Id).ToHashSet() ?? new HashSet<Guid>();
        
        var choiceIdsToDelete = existingChoiceIds.Except(newChoiceIds).ToList();
    
        foreach (var choiceId in choiceIdsToDelete)
        {
            await DeleteChoiceAsync(choiceId);
        }
        
        if (menu.Choices != null && menu.Choices.Any())
        {
            foreach (var choice in menu.Choices)
            {
                await AddOrUpdateChoiceAsync(choice, ctx);
            }
        }

        return menu.Id;
    }
    
    private async Task<Guid> AddOrUpdateChoiceAsync(ChoiceDbO choice, LoadContext ctx)
    {
        var exists = await CheckChoiceExistsAsync(choice.Id);

        if (exists)
            await UpdateChoiceAsync(choice);
        else
            await AddChoiceAsync(choice);
        
        if (choice.NextLabel != null)
        {
            await labelRepo.Value.AddOrUpdateFullAsync(choice.NextLabel, ctx);
        }

        return choice.Id;
    }
    
    public async Task<HashSet<Guid>> GetChoiceIdsByMenuIdAsync(Guid menuId)
    {
        const string sql = "SELECT id FROM \"Choices\" WHERE menu_id = @MenuId";
        var ids = await QueryAsync<Guid>(sql, new { MenuId = menuId });
        return ids.ToHashSet();
    }
}