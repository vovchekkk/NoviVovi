using NoviVovi.Infrastructure.DatabaseObjects.Choices;
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

        const string menuSql = @"SELECT id AS Id
                             FROM ""Menus"" WHERE id = @MenuId";

        var menu = await QueryFirstOrDefaultAsync<MenuDbO>(menuSql, new { MenuId = menuId });
        if (menu == null) return null;

        ctx.Menus[menuId] = menu;

        const string choicesSql = @"SELECT id AS Id, menu_id AS MenuId, next_label_id AS NextLabelId, text AS Text
                               FROM ""Choices"" WHERE menu_id = @MenuId ORDER BY Id";

        var choices = (await QueryAsync<ChoiceDbO>(choicesSql, new { MenuId = menuId })).ToList();

        foreach (var choice in choices)
        {
            choice.NextLabel = await labelRepo.Value.GetFullByIdAsync(choice.NextLabelId, ctx);
        }

        menu.Choices = choices;
        return menu;
    }

    private async Task AddAsync(MenuDbO menu)
    {
        const string sql = @"
            INSERT INTO ""Menus"" (id)
            VALUES (@Id)";

        await ExecuteAsync(sql, menu);
    }

    private async Task AddChoiceAsync(ChoiceDbO choice)
    {
        const string sql = @"
            INSERT INTO ""Choices"" (id , menu_id, next_label_id, text)
            VALUES (@Id, @MenuId, @NextLabelId, @Text)";

        await ExecuteAsync(sql, choice);
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
                text = @Text
            WHERE id = @Id";

        await ExecuteAsync(sql, choice);
    }

    // public async Task UpdateAsync(MenuDbO menu)
    // {
    //     const string sql = @"
    //         UPDATE ""Menus""
    //         SET name = @Name,
    //             text = @Text,
    //             description = @Description
    //         WHERE id = @Id";
    //
    //     await ExecuteAsync(sql, menu);
    // }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = @"DELETE FROM ""Menus"" WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
    }

    private async Task<bool> CheckIfExistsAsync(Guid id)
    {
        const string sql = @"SELECT 1 FROM ""Menus"" WHERE id = @Id LIMIT 1";
        var result = await QueryFirstOrDefaultAsync<int?>(sql, new { Id = id });
        return result.HasValue;
    }

    private async Task<bool> CheckChoiceExistsAsync(Guid id)
    {
        const string sql = @"SELECT 1 FROM ""Choices"" WHERE id = @Id LIMIT 1";
        var result = await QueryFirstOrDefaultAsync<int?>(sql, new { Id = id });
        return result.HasValue;
    }
    
    public async Task<Guid> AddOrUpdateFullAsync(MenuDbO menu, LoadContext? ctx = null)
    {
        ctx ??= new LoadContext();
        
        if (!ctx.Menus.TryAdd(menu.Id, menu))
            return menu.Id;

        var exists = await CheckIfExistsAsync(menu.Id);

        if (!exists)
            await AddAsync(menu);
        
        var existingChoiceIds = await GetChoiceIdsByMenuIdAsync(menu.Id);
        var newChoiceIds = menu.Choices.Select(c => c.Id).ToHashSet();
        
        var choiceIdsToDelete = existingChoiceIds.Except(newChoiceIds).ToList();
    
        foreach (var choiceId in choiceIdsToDelete)
        {
            await DeleteChoiceAsync(choiceId);
        }
        
        if (menu.Choices.Count != 0)
        {
            foreach (var choice in menu.Choices)
            {
                await AddOrUpdateChoiceAsync(choice, ctx);
            }
        }

        return menu.Id;
    }

    public async Task<Guid> AddOrUpdateChoiceAsync(ChoiceDbO choice, LoadContext ctx)
    {
        var exists = await CheckChoiceExistsAsync(choice.Id);
        
        if (choice.NextLabel != null)
        {
            await labelRepo.Value.AddOrUpdateFullAsync(choice.NextLabel, ctx);
        }
        
        if (exists)
            await UpdateChoiceAsync(choice);
        else
            await AddChoiceAsync(choice);

        return choice.Id;
    }

    private async Task<HashSet<Guid>> GetChoiceIdsByMenuIdAsync(Guid menuId)
    {
        const string sql = "SELECT id FROM \"Choices\" WHERE menu_id = @MenuId";
        var ids = await QueryAsync<Guid>(sql, new { MenuId = menuId });
        return ids.ToHashSet();
    }
}