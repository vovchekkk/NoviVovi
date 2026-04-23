using Dapper;
using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.DatabaseObjects.Novels;
using NoviVovi.Infrastructure.Repositories;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;
using NoviVovi.Infrastructure.Tests.Tests;
using Npgsql;

namespace NoviVovi.Infrastructure.Tests.Database;

[Collection("Sequential")]
public class NovelRepoTest : IAsyncLifetime
{
    private readonly IServiceProvider provider;
    private readonly string connectionString;
    private readonly Dictionary<string, HashSet<Guid>> idsToDelete = new();
    private readonly List<string> deleteOrder =
    [
        "Novels", "Characters", "Labels"
    ];
    
    private readonly INovelDbORepository novelRepo;
    private readonly ICharacterDbORepository characterRepo;
    private readonly ILabelDbORepository labelRepo;
    
    private NpgsqlConnection _connection = null!;
    
    public NovelRepoTest()
    {
        provider = TestHelper.CreateProvider();
        novelRepo = provider.GetRequiredService<INovelDbORepository>();
        characterRepo = provider.GetRequiredService<ICharacterDbORepository>();
        labelRepo = provider.GetRequiredService<ILabelDbORepository>();
        var options = provider.GetRequiredService<DatabaseOptions>();
        connectionString = options.ConnectionString;
        
        foreach (var table in deleteOrder)
        {
            idsToDelete[table] = new HashSet<Guid>();
        }
    }
    
    public async Task InitializeAsync()
    {
        _connection = new NpgsqlConnection(connectionString);
        await _connection.OpenAsync();
    }
    
    private void TrackId(string tableName, Guid id)
    {
        if (idsToDelete.ContainsKey(tableName))
            idsToDelete[tableName].Add(id);
    }
    
    private async Task DeleteFromTableAsync(string tableName, IEnumerable<Guid> ids)
    {
        var idList = ids.ToList();
        if (!idList.Any()) return;
        var deleteSql = $"DELETE FROM \"{tableName}\" WHERE id = ANY(@Ids)";
        await _connection.ExecuteAsync(deleteSql, new { Ids = idList });
    }
    
    // ========== Вспомогательные конструкторы ==========
    
    private NovelDbO CreateNovel(string title = "Test Novel", bool isPublic = true)
    {
        var novel = new NovelDbO
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = "Test description",
            IsPublic = isPublic,
            CreatedAt = DateTime.UtcNow,
            EditedAt = DateTime.UtcNow,
            Characters = new List<CharacterDbO>(),
            Labels = new List<LabelDbO>()
        };
        TrackId("Novels", novel.Id);
        return novel;
    }
    
    private CharacterDbO CreateCharacter(string name = "Test Char", string color = "000000")
    {
        var character = new CharacterDbO
        {
            Id = Guid.NewGuid(),
            NovelId = Guid.Empty, // будет установлен позже при сохранении
            Name = name,
            NameColor = color,
            Description = "char desc"
        };
        TrackId("Characters", character.Id);
        return character;
    }
    
    private LabelDbO CreateLabel(string labelName = "test_label")
    {
        var label = new LabelDbO
        {
            Id = Guid.NewGuid(),
            NovelId = Guid.Empty,
            LabelName = labelName
        };
        TrackId("Labels", label.Id);
        return label;
    }
    
    // ========== Тесты ==========
    
    [Fact]
    public async Task TestCreateNovel()
    {
        var novel = CreateNovel("Brand New Novel");
        
        var result = await novelRepo.AddOrUpdateFullAsync(novel);
        
        Assert.Equal(novel.Id, result);
        
        const string sql = "SELECT title, description, is_public FROM \"Novels\" WHERE id = @Id";
        var dbNovel = await _connection.QueryFirstOrDefaultAsync<(string title, string description, bool isPublic)>(sql, new { Id = novel.Id });
        Assert.NotNull(dbNovel);
        Assert.Equal("Brand New Novel", dbNovel.title);
        Assert.Equal("Test description", dbNovel.description);
        Assert.True(dbNovel.isPublic);
    }
    
    [Fact]
    public async Task TestGetNovelById()
    {
        var novel = CreateNovel("Get Novel");
        await novelRepo.AddOrUpdateFullAsync(novel);
        
        var ctx = new LoadContext();
        var result = await novelRepo.GetFullByIdAsync(novel.Id, ctx);
        
        Assert.NotNull(result);
        Assert.Equal(novel.Id, result.Id);
        Assert.Equal(novel.Title, result.Title);
        Assert.Equal(novel.Description, result.Description);
        Assert.Equal(novel.IsPublic, result.IsPublic);
    }
    
    [Fact]
    public async Task TestUpdateNovel()
    {
        var novel = CreateNovel("Original Title");
        await novelRepo.AddOrUpdateFullAsync(novel);
        
        novel.Title = "Updated Title";
        novel.Description = "Updated description";
        novel.IsPublic = false;
        await novelRepo.AddOrUpdateFullAsync(novel);
        
        const string sql = "SELECT title, description, is_public FROM \"Novels\" WHERE id = @Id";
        var dbNovel = await _connection.QueryFirstOrDefaultAsync<(string title, string description, bool isPublic)>(sql, new { Id = novel.Id });
        Assert.Equal("Updated Title", dbNovel.title);
        Assert.Equal("Updated description", dbNovel.description);
        Assert.False(dbNovel.isPublic);
    }
    
    [Fact]
    public async Task TestDeleteNovel()
    {
        var novel = CreateNovel("To Delete");
        await novelRepo.AddOrUpdateFullAsync(novel);
        
        var before = await _connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Novels\" WHERE id = @Id", new { Id = novel.Id });
        Assert.Equal(1, before);
        
        await novelRepo.DeleteAsync(novel.Id);
        idsToDelete["Novels"].Remove(novel.Id);
        
        var after = await _connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Novels\" WHERE id = @Id", new { Id = novel.Id });
        Assert.Equal(0, after);
    }
    
    [Fact]
    public async Task TestAddCharacterToNovel()
    {
        var novel = CreateNovel("Novel With Character");
        await novelRepo.AddOrUpdateFullAsync(novel);
        
        var character = CreateCharacter("Alice");
        novel.Characters = new List<CharacterDbO> { character };
        
        await novelRepo.AddOrUpdateFullAsync(novel);
        
        // Проверяем, что персонаж сохранился и связан с романом
        const string sql = "SELECT novel_id FROM \"Characters\" WHERE id = @Id";
        var dbNovelId = await _connection.ExecuteScalarAsync<Guid>(sql, new { Id = character.Id });
        Assert.Equal(novel.Id, dbNovelId);
        
        // Проверяем, что при загрузке романа персонаж подтягивается
        var ctx = new LoadContext();
        var loadedNovel = await novelRepo.GetFullByIdAsync(novel.Id, ctx);
        Assert.Single(loadedNovel.Characters);
        Assert.Equal(character.Id, loadedNovel.Characters[0].Id);
    }
    
    [Fact]
    public async Task TestAddLabelToNovel()
    {
        var novel = CreateNovel("Novel With Label");
        await novelRepo.AddOrUpdateFullAsync(novel);
        
        var label = CreateLabel("start_label");
        novel.Labels = [label];
        
        await novelRepo.AddOrUpdateFullAsync(novel);
        
        const string sql = "SELECT novel_id FROM \"Labels\" WHERE id = @Id";
        var dbNovelId = await _connection.ExecuteScalarAsync<Guid>(sql, new { Id = label.Id });
        Assert.Equal(novel.Id, dbNovelId);
        
        var ctx = new LoadContext();
        var loadedNovel = await novelRepo.GetFullByIdAsync(novel.Id, ctx);
        Assert.Single(loadedNovel.Labels);
        Assert.Equal(label.Id, loadedNovel.Labels[0].Id);
    }
    
    [Fact]
    public async Task TestUpdateNovelWithCharactersAndLabels()
    {
        var novel = CreateNovel("Original");
        var char1 = CreateCharacter("Char1");
        var char2 = CreateCharacter("Char2");
        var label1 = CreateLabel("Label1");
        var label2 = CreateLabel("Label2");
        
        novel.Characters = new List<CharacterDbO> { char1, char2 };
        novel.Labels = new List<LabelDbO> { label1, label2 };
        await novelRepo.AddOrUpdateFullAsync(novel);
        
        // Удаляем char2 и label2, добавляем нового персонажа
        var char3 = CreateCharacter("Char3");
        novel.Characters = new List<CharacterDbO> { char1, char3 };
        novel.Labels = new List<LabelDbO> { label1 };
        
        await novelRepo.AddOrUpdateFullAsync(novel);
        
        // Проверяем, что char2 удалён из БД, а char3 добавлен
        var char2Exists = await _connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Characters\" WHERE id = @Id", new { Id = char2.Id });
        var char3Exists = await _connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Characters\" WHERE id = @Id", new { Id = char3.Id });
        var label2Exists = await _connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Labels\" WHERE id = @Id", new { Id = label2.Id });
        
        Assert.Equal(0, char2Exists);
        Assert.Equal(1, char3Exists);
        Assert.Equal(0, label2Exists);
        
        // Получаем роман и проверяем списки
        var ctx = new LoadContext();
        var loadedNovel = await novelRepo.GetFullByIdAsync(novel.Id, ctx);
        Assert.Equal(2, loadedNovel.Characters.Count);
        Assert.Single(loadedNovel.Labels);
        Assert.Contains(loadedNovel.Characters, c => c.Id == char1.Id);
        Assert.Contains(loadedNovel.Characters, c => c.Id == char3.Id);
        Assert.Contains(loadedNovel.Labels, l => l.Id == label1.Id);
    }
    
    [Fact]
    public async Task TestRemoveCharacterAndLabelFromNovel()
    {
        var novel = CreateNovel("Remove Test");
        var charToRemove = CreateCharacter("ToRemove");
        var labelToRemove = CreateLabel("ToRemoveLabel");
        novel.Characters = new List<CharacterDbO> { charToRemove };
        novel.Labels = new List<LabelDbO> { labelToRemove };
        await novelRepo.AddOrUpdateFullAsync(novel);
        
        // Удаляем из списков
        novel.Characters = new List<CharacterDbO>();
        novel.Labels = new List<LabelDbO>();
        await novelRepo.AddOrUpdateFullAsync(novel);
        
        // Проверяем, что персонаж и лейбл удалены
        var charExists = await _connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Characters\" WHERE id = @Id", new { Id = charToRemove.Id });
        var labelExists = await _connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Labels\" WHERE id = @Id", new { Id = labelToRemove.Id });
        Assert.Equal(0, charExists);
        Assert.Equal(0, labelExists);
        
        // Убираем из трекинга, чтобы DisposeAsync не пытался удалить их снова
        idsToDelete["Characters"].Remove(charToRemove.Id);
        idsToDelete["Labels"].Remove(labelToRemove.Id);
    }
    
    [Fact]
    public async Task TestGetAllNovels()
    {
        var novel1 = CreateNovel("Novel A");
        var novel2 = CreateNovel("Novel B");
        await novelRepo.AddOrUpdateFullAsync(novel1);
        await novelRepo.AddOrUpdateFullAsync(novel2);
        
        var allNovels = (await novelRepo.GetAllFullAsync(onlyPublic: true)).ToList();
        Assert.Contains(allNovels, n => n.Id == novel1.Id);
        Assert.Contains(allNovels, n => n.Id == novel2.Id);
        
        // Проверяем фильтр onlyPublic
        var privateNovel = CreateNovel("Private Novel", false);
        await novelRepo.AddOrUpdateFullAsync(privateNovel);
        var publicOnly = (await novelRepo.GetAllFullAsync(onlyPublic: true)).ToList();
        Assert.DoesNotContain(publicOnly, n => n.Id == privateNovel.Id);
        
        var allIncludingPrivate = (await novelRepo.GetAllFullAsync(onlyPublic: false)).ToList();
        Assert.Contains(allIncludingPrivate, n => n.Id == privateNovel.Id);
    }
    
    [Fact]
    public async Task TestCascadeDeleteWhenNovelDeleted()
    {
        var novel = CreateNovel("Cascade Novel");
        var character = CreateCharacter("CascadeChar");
        var label = CreateLabel("CascadeLabel");
        novel.Characters = new List<CharacterDbO> { character };
        novel.Labels = new List<LabelDbO> { label };
        await novelRepo.AddOrUpdateFullAsync(novel);
        
        // Проверяем, что персонаж и лейбл существуют
        var charBefore = await _connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Characters\" WHERE id = @Id", new { Id = character.Id });
        var labelBefore = await _connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Labels\" WHERE id = @Id", new { Id = label.Id });
        Assert.Equal(1, charBefore);
        Assert.Equal(1, labelBefore);
        
        // Удаляем роман
        await novelRepo.DeleteAsync(novel.Id);
        idsToDelete["Novels"].Remove(novel.Id);
        
        // Проверяем, что персонаж и лейбл удалены каскадно
        var charAfter = await _connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Characters\" WHERE id = @Id", new { Id = character.Id });
        var labelAfter = await _connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Labels\" WHERE id = @Id", new { Id = label.Id });
        Assert.Equal(0, charAfter);
        Assert.Equal(0, labelAfter);
        
        // Убираем из трекинга
        idsToDelete["Characters"].Remove(character.Id);
        idsToDelete["Labels"].Remove(label.Id);
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
        await _connection.DisposeAsync();
    }
}