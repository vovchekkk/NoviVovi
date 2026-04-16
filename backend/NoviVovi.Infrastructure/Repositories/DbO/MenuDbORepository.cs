using NoviVovi.Infrastructure.DatabaseObjects.Choices;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

namespace NoviVovi.Infrastructure.Repositories.DbO;

public class MenuDbORepository(
    DatabaseOptions options/*,
    ILabelDbORepository labelRepo*/
) : BaseRepository(options), IMenuDbORepository
{
    public async Task<MenuDbO?> GetFullByIdAsync(Guid menuId)
    {
        throw new NotImplementedException();
        // const string menuSql = @"SELECT id AS Id, name AS Name, text AS Text, description AS Description
        //                          FROM ""Menus"" WHERE id = @MenuId";
        // var menu = await QueryFirstOrDefaultAsync<MenuDbO>(menuSql, new { MenuId = menuId });
        // if (menu == null) return null;
        //
        // const string choicesSql = @"SELECT id AS Id, menu_id AS MenuId, next_label_id AS NextLabelId,
        //                                    name AS Name, text AS Text
        //                             FROM ""Choices"" WHERE menu_id = @MenuId ORDER BY name";
        // var choices = (await QueryAsync<ChoiceDbO>(choicesSql, new { MenuId = menuId })).ToList();
        //
        // foreach (var choice in choices)
        // {
        //     choice.NextLabel = await labelRepo.GetFullByIdAsync(choice.NextLabelId);
        // }
        //
        // menu.Choices = choices;
        // return menu;
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

    public async Task UpdateChoiceAsync(ChoiceDbO choice)
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

    public async Task AddFullAsync(MenuDbO stepMenu) //TODO! AddOrUpdate
    {
        foreach (var choice in stepMenu.Choices)
        {
            await AddChoiceAsync(choice);
        }

        await AddAsync(stepMenu);
    }
}