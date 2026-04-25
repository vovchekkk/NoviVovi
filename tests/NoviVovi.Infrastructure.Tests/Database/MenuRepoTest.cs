using Dapper;
using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Infrastructure.DatabaseObjects.Choices;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.Repositories;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;
using NoviVovi.Infrastructure.Tests.Tests;
using Npgsql;

namespace NoviVovi.Infrastructure.Tests.Database;


[Collection("Sequential")]
public class MenuRepoTest : IAsyncLifetime
{
    private readonly IServiceProvider provider;
    private readonly string connectionString;
    private readonly Dictionary<string, HashSet<Guid>> idsToDelete = new();
    private readonly List<string> deleteOrder = new()
    {
        "Choices", "Menus", "Labels", "Novels"
    };
    
    
    private Guid novelId;
    private Guid labelId;
    
    private readonly ILabelDbORepository labelRepo;
    private readonly IMenuDbORepository menuRepo;
    
    public MenuRepoTest()
    {
        provider = TestHelper.CreateProvider();
        labelRepo = provider.GetRequiredService<ILabelDbORepository>();
        menuRepo = provider.GetRequiredService<IMenuDbORepository>();
        var options = provider.GetRequiredService<DatabaseOptions>();
        connectionString = options.ConnectionString;
        
        foreach (var table in deleteOrder)
        {
            idsToDelete[table] = [];
        }
    }
    
    public async Task InitializeAsync()
    {
        novelId = Guid.NewGuid();
        TrackId("Novels", novelId);
        
        const string insertNovelSql = @"
            INSERT INTO ""Novels"" (id, title, is_public, created_at, edited_at)
            VALUES (@Id, @Title, @IsPublic, NOW(), NOW())";
        
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.ExecuteAsync(insertNovelSql, new
        {
            Id = novelId,
            Title = "Test Novel for Labels",
            IsPublic = true
        });
        
        
        labelId = Guid.NewGuid();
        TrackId("Labels", labelId);
        
        const string insertLabelSql = @"
            INSERT INTO ""Labels"" (id, novel_id, label_name)
            VALUES (@Id, @NovelId, @LabelName)";
        
        await conn.ExecuteAsync(insertLabelSql, new
        {
            Id = labelId,
            NovelId = novelId,
            LabelName = "test_label_for_menu"
        });
    }
    
    private LabelDbO CreateLabel(string labelName = "test_label")
    {
        var label = new LabelDbO
        {
            Id = Guid.NewGuid(),
            NovelId = novelId,
            LabelName = labelName
        };
        TrackId("Labels", label.Id);
        return label;
    }

    private MenuDbO CreateMenu()
    {
        var menu = new MenuDbO
        {
            Id = Guid.NewGuid(),
        };
        TrackId("Menus", menu.Id);
        return menu;
    }

    private ChoiceDbO CreateChoice(Guid menuId, LabelDbO nextLabel)
    {
        var choice = new ChoiceDbO
        {
            Id = Guid.NewGuid(),
            MenuId = menuId,
            NextLabel = nextLabel,
            NextLabelId = nextLabel.Id,
            Text = "abobiki"
        };
        TrackId("Choices", choice.Id);
        return choice;
    }
    
    private async Task DeleteFromTableAsync(string tableName, IEnumerable<Guid> ids)
    {
        if (!ids.Any()) return;
        
        var deleteSql = $"DELETE FROM \"{tableName}\" WHERE id = ANY(@Ids)";
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.ExecuteAsync(deleteSql, new { Ids = ids.ToArray() });
    }
    
    private void TrackId(string tableName, Guid id)
    {
        if (idsToDelete.ContainsKey(tableName))
            idsToDelete[tableName].Add(id);
    }
    
    [Fact]
    public async Task TestGetMenuById()
    {
        var menu = CreateMenu();
        
        const string insertSql = @"
            INSERT INTO ""Menus"" (id)
            VALUES (@Id)";
        
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.ExecuteAsync(insertSql, menu);
        
        var ctx = new LoadContext();
        var result = await menuRepo.GetFullByIdAsync(menu.Id, ctx);
        
        Assert.NotNull(result);
        Assert.Equal(menu.Id, result.Id);
    }
    
    [Fact]
    public async Task TestCreateMenu()
    {
        var menu = CreateMenu();
        
        var result = await menuRepo.AddOrUpdateFullAsync(menu, new LoadContext());
        
        Assert.Equal(menu.Id, result);
        
        const string sql = "SELECT COUNT(*) FROM \"Menus\" WHERE id = @Id";
        await using var conn = new NpgsqlConnection(connectionString);
        var count = await conn.ExecuteScalarAsync<int>(sql, new { Id = menu.Id });
        Assert.Equal(1, count);
    }
    
    [Fact]
    public async Task TestDeleteMenu()
    {
        var menu = CreateMenu();
        await menuRepo.AddOrUpdateFullAsync(menu, new LoadContext());
        
        const string countSql = "SELECT COUNT(*) FROM \"Menus\" WHERE id = @Id";
        await using var conn = new NpgsqlConnection(connectionString);
        
        var before = await conn.ExecuteScalarAsync<int>(countSql, new { Id = menu.Id });
        Assert.Equal(1, before);
        
        await menuRepo.DeleteAsync(menu.Id);
        idsToDelete["Menus"].Remove(menu.Id);
        
        var after = await conn.ExecuteScalarAsync<int>(countSql, new { Id = menu.Id });
        Assert.Equal(0, after);
    }
    
    [Fact]
    public async Task TestGetChoiceById()
    {
        var menu = CreateMenu();
        await menuRepo.AddOrUpdateFullAsync(menu, new LoadContext());
        
        var label = CreateLabel("choice_label_for_get");
        await labelRepo.AddOrUpdateFullAsync(label);
        
        var choice = CreateChoice(menu.Id, label);
        
        const string insertSql = @"
            INSERT INTO ""Choices"" (id, menu_id, next_label_id, text)
            VALUES (@Id, @MenuId, @NextLabelId, @Text)";
        
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.ExecuteAsync(insertSql, choice);
        
        var ctx = new LoadContext();
        var resultMenu = await menuRepo.GetFullByIdAsync(menu.Id, ctx);
        
        Assert.NotNull(resultMenu);
        Assert.Single(resultMenu.Choices);
        
        var resultChoice = resultMenu.Choices[0];
        Assert.Equal(choice.Id, resultChoice.Id);
        Assert.Equal(choice.MenuId, resultChoice.MenuId);
        Assert.Equal(choice.Text, resultChoice.Text);
        Assert.NotNull(resultChoice.NextLabel);
        Assert.Equal(label.Id, resultChoice.NextLabel.Id);
    }
    
    [Fact]
    public async Task TestCreateChoice()
    {
        var menu = CreateMenu();
        await menuRepo.AddOrUpdateFullAsync(menu, new LoadContext());
        
        var label = CreateLabel("choice_label");
        
        var choice = CreateChoice(menu.Id, label);
        
        var result = await menuRepo.AddOrUpdateChoiceAsync(choice, new LoadContext());
        
        Assert.Equal(choice.Id, result);
        
        const string countSql = "SELECT COUNT(*) FROM \"Choices\" WHERE id = @Id";
        await using var conn = new NpgsqlConnection(connectionString);
        var count = await conn.ExecuteScalarAsync<int>(countSql, new { choice.Id });
        Assert.Equal(1, count);
        
        const string selectSql = @"
            SELECT menu_id, next_label_id, text FROM ""Choices"" WHERE id = @Id";
        var dbChoice = await conn.QueryFirstOrDefaultAsync<(Guid menuId, Guid nextLabelId, string text)>(
            selectSql, new { choice.Id });
        
        Assert.Equal(choice.MenuId, dbChoice.menuId);
        Assert.Equal(choice.NextLabelId, dbChoice.nextLabelId);
        Assert.Equal(choice.Text, dbChoice.text);
    }

    [Fact]
    public async Task TestUpdateChoice()
    {
        var menu = CreateMenu();
        await menuRepo.AddOrUpdateFullAsync(menu, new LoadContext());

        var originalLabel = CreateLabel("original_label_for_choice");
        var newLabel = CreateLabel("new_label_for_choice");
        await labelRepo.AddOrUpdateFullAsync(originalLabel);
        await labelRepo.AddOrUpdateFullAsync(newLabel);

        var choice = CreateChoice(menu.Id, originalLabel);
        choice.Text = "original_choice_text";

        menu.Choices = [choice];
        await menuRepo.AddOrUpdateFullAsync(menu, new LoadContext());
        
        await using var conn = new NpgsqlConnection(connectionString);
        var before = await conn.QueryFirstOrDefaultAsync<(string text, Guid nextLabelId)>(
            "SELECT text, next_label_id FROM \"Choices\" WHERE id = @Id",
            new { choice.Id });
        
        Assert.Equal("original_choice_text", before.text);
        Assert.Equal(originalLabel.Id, before.nextLabelId);
        
        choice.Text = "updated_choice_text";
        choice.NextLabel = newLabel;
        choice.NextLabelId = newLabel.Id;

        await menuRepo.AddOrUpdateFullAsync(menu, new LoadContext());
        
        var after = await conn.QueryFirstOrDefaultAsync<(string text, Guid nextLabelId)>(
            "SELECT text, next_label_id FROM \"Choices\" WHERE id = @Id",
            new { choice.Id });
        
        Assert.Equal("updated_choice_text", after.text);
        Assert.Equal(newLabel.Id, after.nextLabelId);
        
        var count = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM \"Choices\" WHERE menu_id = @MenuId",
            new { MenuId = menu.Id });
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task TestDeleteChoice()
    {
        var menu = CreateMenu();
        await menuRepo.AddOrUpdateFullAsync(menu, new LoadContext());
        
        var label = CreateLabel("choice_label_for_delete");
        await labelRepo.AddOrUpdateFullAsync(label);
        
        var choice = CreateChoice(menu.Id, label);
        await menuRepo.AddOrUpdateChoiceAsync(choice, new LoadContext());
        
        const string countSql = "SELECT COUNT(*) FROM \"Choices\" WHERE id = @Id";
        await using var conn = new NpgsqlConnection(connectionString);
        
        var before = await conn.ExecuteScalarAsync<int>(countSql, new { choice.Id });
        Assert.Equal(1, before);
        
        await menuRepo.DeleteChoiceAsync(choice.Id);
        
        var after = await conn.ExecuteScalarAsync<int>(countSql, new { choice.Id });
        Assert.Equal(0, after);
    }
    
    [Fact]
    public async Task TestGetMenuWithChoices()
    {
        var menu = CreateMenu();
        await menuRepo.AddOrUpdateFullAsync(menu, new LoadContext());
        
        var label1 = CreateLabel("label_1");
        var label2 = CreateLabel("label_2");
        await labelRepo.AddOrUpdateFullAsync(label1);
        await labelRepo.AddOrUpdateFullAsync(label2);
        
        var choice1 = CreateChoice(menu.Id, label1);
        var choice2 = CreateChoice(menu.Id, label2);
        
        await menuRepo.AddOrUpdateChoiceAsync(choice1, new LoadContext());
        await menuRepo.AddOrUpdateChoiceAsync(choice2, new LoadContext());
        
        var ctx = new LoadContext();
        var result = await menuRepo.GetFullByIdAsync(menu.Id, ctx);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Choices.Count);
        Assert.Contains(result.Choices, c => c.Id == choice1.Id);
        Assert.Contains(result.Choices, c => c.Id == choice2.Id);
        
        foreach (var choice in result.Choices)
        {
            Assert.NotNull(choice.NextLabel);
        }
    }
    
    [Fact]
    public async Task TestCreateMenuWithChoices()
    {
        var menu = CreateMenu();
        var label1 = CreateLabel("new_label_1");
        var label2 = CreateLabel("new_label_2");
        
        await labelRepo.AddOrUpdateFullAsync(label1);
        await labelRepo.AddOrUpdateFullAsync(label2);
        
        var choice1 = CreateChoice(menu.Id, label1);
        choice1.Text = "aboba1";
        var choice2 = CreateChoice(menu.Id, label2);
        choice2.Text = "aboba2";
        menu.Choices = [choice1, choice2];
        
        await menuRepo.AddOrUpdateFullAsync(menu, new LoadContext());
        
        const string countSql = "SELECT COUNT(*) FROM \"Choices\" WHERE menu_id = @MenuId";
        await using var conn = new NpgsqlConnection(connectionString);
        var count = await conn.ExecuteScalarAsync<int>(countSql, new { MenuId = menu.Id });
        Assert.Equal(2, count);
        
        const string selectSql = @"
            SELECT text, next_label_id FROM ""Choices"" WHERE menu_id = @MenuId";
        var dbChoices = await conn.QueryAsync<(string text, Guid nextLabelId)>(selectSql, new { MenuId = menu.Id });
        var choicesList = dbChoices.ToList();
        
        Assert.Contains(choicesList, c => c.text == "aboba1");
        Assert.Contains(choicesList, c => c.text == "aboba2");
    }
    
    [Fact]
    public async Task TestUpdateMenuWithChoices()
    {
        var menu = CreateMenu();
        var originalLabel = CreateLabel("original_label");
        await labelRepo.AddOrUpdateFullAsync(originalLabel);
        
        var originalChoice = CreateChoice(menu.Id, originalLabel);
        menu.Choices = [originalChoice];
        await menuRepo.AddOrUpdateFullAsync(menu, new LoadContext());
        
        var newLabel = CreateLabel("new_label_for_updated_choice");
        await labelRepo.AddOrUpdateFullAsync(newLabel);
        
        await using var conn = new NpgsqlConnection(connectionString);
        
        originalChoice.Text = "updated_text";
        originalChoice.NextLabel = newLabel;
        originalChoice.NextLabelId = newLabel.Id;
        
        var newChoice = CreateChoice(menu.Id, originalLabel);
        newChoice.Text = "brand_new_choice";
        menu.Choices.Add(newChoice);
        
        await menuRepo.AddOrUpdateFullAsync(menu, new LoadContext());
        
        const string choicesSql = @"
            SELECT text, next_label_id FROM ""Choices"" WHERE menu_id = @MenuId";
        var dbChoices = await conn.QueryAsync<(string text, Guid nextLabelId)>(choicesSql, new { MenuId = menu.Id });
        var choicesList = dbChoices.ToList();
        
        Assert.Equal(2, choicesList.Count);
        Assert.Contains(choicesList, c => c is { text: "updated_text" } && c.nextLabelId == newLabel.Id);
        Assert.Contains(choicesList, c => c.text == "brand_new_choice");
    }
    
    [Fact]
    public async Task TestDeleteMenuWithChoices()
    {
        var menu = CreateMenu();
        var label = CreateLabel("label_for_choices");
        await labelRepo.AddOrUpdateFullAsync(label);
        
        var choice1 = CreateChoice(menu.Id, label);
        var choice2 = CreateChoice(menu.Id, label);
        
        await menuRepo.AddOrUpdateFullAsync(menu, new LoadContext());
        await menuRepo.AddOrUpdateChoiceAsync(choice1, new LoadContext());
        await menuRepo.AddOrUpdateChoiceAsync(choice2, new LoadContext());
        
        await using var conn = new NpgsqlConnection(connectionString);
        
        var menuBefore = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Menus\" WHERE id = @Id", new { Id = menu.Id });
        var choicesBefore = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Choices\" WHERE menu_id = @MenuId", new { MenuId = menu.Id });
        Assert.Equal(1, menuBefore);
        Assert.Equal(2, choicesBefore);
        
        await menuRepo.DeleteAsync(menu.Id);
        idsToDelete["Menus"].Remove(menu.Id);
        
        var menuAfter = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Menus\" WHERE id = @Id", new { Id = menu.Id });
        var choicesAfter = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Choices\" WHERE menu_id = @MenuId", new { MenuId = menu.Id });
        
        Assert.Equal(0, menuAfter);
        Assert.Equal(0, choicesAfter);

        idsToDelete["Choices"].Remove(choice1.Id);
        idsToDelete["Choices"].Remove(choice2.Id);
    }

    [Fact]
    public async Task TestUpdateMenuRemovesObsoleteChoices()
    {
        var menu = CreateMenu();
        var label1 = CreateLabel("label_1");
        var label2 = CreateLabel("label_2");
        await labelRepo.AddOrUpdateFullAsync(label1);
        await labelRepo.AddOrUpdateFullAsync(label2);

        var choice1 = CreateChoice(menu.Id, label1);
        var choice2 = CreateChoice(menu.Id, label2);
        var choice3 = CreateChoice(menu.Id, label1);

        menu.Choices = [choice1, choice2, choice3];
        await menuRepo.AddOrUpdateFullAsync(menu, new LoadContext());
        
        await using var conn = new NpgsqlConnection(connectionString);
        var beforeCount = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM \"Choices\" WHERE menu_id = @MenuId",
            new { MenuId = menu.Id });
        Assert.Equal(3, beforeCount);
        
        menu.Choices = [choice1, choice3];
        await menuRepo.AddOrUpdateFullAsync(menu, new LoadContext());
        
        var afterCount = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM \"Choices\" WHERE menu_id = @MenuId",
            new { MenuId = menu.Id });
        Assert.Equal(2, afterCount);
        
        var choice2Exists = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM \"Choices\" WHERE id = @Id",
            new { Id = choice2.Id });
        Assert.Equal(0, choice2Exists);
        
        var choice1Exists = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM \"Choices\" WHERE id = @Id",
            new { Id = choice1.Id });
        Assert.Equal(1, choice1Exists);

        var choice3Exists = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM \"Choices\" WHERE id = @Id",
            new { Id = choice3.Id });
        Assert.Equal(1, choice3Exists);
    }

    public async Task DisposeAsync()
    {
        foreach (var tableName in deleteOrder)
        {
            if (idsToDelete.TryGetValue(tableName, out var ids) && ids.Any())
            {
                await DeleteFromTableAsync(tableName, ids);
            }
        }
    }
}