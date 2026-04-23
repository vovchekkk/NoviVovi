using Dapper;
using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.Repositories;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;
using NoviVovi.Infrastructure.Tests.Tests;
using Npgsql;

namespace NoviVovi.Infrastructure.Tests.Database; 

[Collection("Sequential")]
public class LabelRepoTest :  IAsyncLifetime  //короче, оно вроде бы работает
{
    private readonly IServiceProvider provider;
    private readonly string connectionString;
    private readonly Dictionary<string, HashSet<Guid>> idsToDelete = new();
    private readonly List<string> deleteOrder = new()
    {
        "StepCharacters", "Steps", "CharacterStates", "Characters",
        "Choices", "Menus", "Replicas", "Backgrounds", "Labels",
        "Images", "Transforms", "Novels"
    };
    
    private Guid novelId;
    
    private readonly ILabelDbORepository labelRepo;
    private readonly IStepDbORepository stepRepo;

    public LabelRepoTest()
    {
        provider = TestHelper.CreateProvider();
        labelRepo = provider.GetRequiredService<ILabelDbORepository>();
        stepRepo = provider.GetRequiredService<IStepDbORepository>();
        var options = provider.GetRequiredService<DatabaseOptions>();
        connectionString = options.ConnectionString;
        
        foreach (var table in deleteOrder)
        {
            idsToDelete[table] = [];
        }
    }
    
    private void TrackId(string tableName, Guid id)
    {
        if (idsToDelete.ContainsKey(tableName))
            idsToDelete[tableName].Add(id);
    }
    
    private async Task DeleteFromTableAsync(string tableName, IEnumerable<Guid> ids)
    {
        if (!ids.Any()) return;
        
        var deleteSql = $"DELETE FROM \"{tableName}\" WHERE id = ANY(@Ids)";
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.ExecuteAsync(deleteSql, new { Ids = ids.ToArray() });
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
    
    private StepDbO CreateStep(LabelDbO label, int order = 0)
    {
        var step = new StepDbO
        {
            Id = Guid.NewGuid(),
            LabelId = label.Id,
            StepOrder = order,
            StepType = "test"
        };
        TrackId("Steps", step.Id);
        return step;
    }
    
    [Fact]
    public async Task TestGetLabelById()
    {
        var label = CreateLabel("test_get_label");
        
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.ExecuteAsync(@"INSERT INTO ""Labels"" (id, novel_id, label_name) 
                              VALUES (@Id, @NovelId, @LabelName)", label);

        
        var ctx = new LoadContext();
        var result = await labelRepo.GetFullByIdAsync(label.Id, ctx);
        
        Assert.NotNull(result);
        Assert.Equal(label.Id, result.Id);
        Assert.Equal(label.LabelName, result.LabelName);
        Assert.Equal(novelId, result.NovelId);
    }
    
    [Fact]
    public async Task TestCreateLabel()
    {
        var label = CreateLabel("brand_new_label");
        var resultId = await labelRepo.AddOrUpdateFullAsync(label);
        
        Assert.Equal(label.Id, resultId);
        
        const string sql = "SELECT COUNT(*) FROM \"Labels\" WHERE id = @Id";
        await using var conn = new NpgsqlConnection(connectionString);
        var count = await conn.ExecuteScalarAsync<int>(sql, new { Id = label.Id });
        Assert.Equal(1, count);
        
        var ctx = new LoadContext();
        var result = await labelRepo.GetFullByIdAsync(label.Id, ctx);
        Assert.NotNull(result);
        Assert.Equal(label.Id, result.Id);
        Assert.Equal(label.LabelName, result.LabelName);
        Assert.Equal(novelId, result.NovelId);
    }
    
    [Fact]
    public async Task TestUpdateLabel()
    {
        var label = CreateLabel("original_name");
        await labelRepo.AddOrUpdateFullAsync(label);
        
        label.LabelName = "updated_name";
        await labelRepo.AddOrUpdateFullAsync(label);
        
        var ctx = new LoadContext();
        var updated = await labelRepo.GetFullByIdAsync(label.Id, ctx);
        Assert.NotNull(updated);
        Assert.Equal("updated_name", updated.LabelName);
    }
    
    [Fact]
    public async Task TestDeleteLabel()
    {
        var label = CreateLabel("to_be_deleted");
        await labelRepo.AddOrUpdateFullAsync(label);
        
        const string countBeforeSql = "SELECT COUNT(*) FROM \"Labels\" WHERE id = @Id";
        await using var conn = new NpgsqlConnection(connectionString);
        var before = await conn.ExecuteScalarAsync<int>(countBeforeSql, new { Id = label.Id });
        
        Assert.Equal(1, before);
        await labelRepo.DeleteAsync(label.Id);
        
        var after = await conn.ExecuteScalarAsync<int>(countBeforeSql, new { Id = label.Id });
        Assert.Equal(0, after);
    }
    
    [Fact]
    public async Task TestGetStepById()
    {
        var label = CreateLabel("label_for_step_get");
        await labelRepo.AddOrUpdateFullAsync(label); 
        
        var step = CreateStep(label, 42);
        var ctx = new LoadContext();
        await stepRepo.AddOrUpdateFullAsync(step, ctx); //TODO: заменить на SQL запрос для создания степа
        
        var result = await stepRepo.GetFullByIdAsync(step.Id, ctx);
        
        const string sql = "SELECT step_order FROM \"Steps\" WHERE id = @Id";
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.ExecuteAsync(sql, new { Id = step.Id });
        
        Assert.NotNull(result);
        Assert.Equal(step.Id, result.Id);
        Assert.Equal(step.StepOrder, result.StepOrder);
        Assert.Equal(label.Id, result.LabelId);
    }
    
    [Fact]
    public async Task TestCreateStep()
    {
        var label = CreateLabel();
        await labelRepo.AddOrUpdateFullAsync(label);
        
        var step = CreateStep(label);
        
        var ctx = new LoadContext();
        await stepRepo.AddOrUpdateFullAsync(step, ctx);
        
        var steps = await stepRepo.GetOrderedByLabelIdAsync(label.Id, ctx);
        Assert.Contains(steps, s => s.Id == step.Id);
        
        const string sql = "SELECT COUNT(*) FROM \"Steps\" WHERE id = @Id";
        await using var conn = new NpgsqlConnection(connectionString);
        var count = await conn.ExecuteScalarAsync<int>(sql, new { Id = step.Id });
        Assert.Equal(1, count);
    }
    
    [Fact]
    public async Task TestUpdateStep()
    {
        var label = CreateLabel();
        await labelRepo.AddOrUpdateFullAsync(label);
        
        var step = CreateStep(label, 1);
        var ctx = new LoadContext();
        await stepRepo.AddOrUpdateFullAsync(step, ctx);
        
        step.StepOrder = 999;
        await stepRepo.AddOrUpdateFullAsync(step, ctx);
        
        var updated = await stepRepo.GetFullByIdAsync(step.Id, ctx);
        Assert.NotNull(updated);
        Assert.Equal(999, updated.StepOrder);
    }
    
    [Fact]
    public async Task TestDeleteStep()
    {
        var label = CreateLabel("label_for_step_delete");
        await labelRepo.AddOrUpdateFullAsync(label);
        
        var step = CreateStep(label);
        var ctx = new LoadContext();
        await stepRepo.AddOrUpdateFullAsync(step, ctx);
        
        const string countSql = "SELECT COUNT(*) FROM \"Steps\" WHERE id = @Id";
        await using var conn = new NpgsqlConnection(connectionString);
        var before = await conn.ExecuteScalarAsync<int>(countSql, new { Id = step.Id });
        Assert.Equal(1, before);
        
        await stepRepo.DeleteAsync(step.Id);
        
        var after = await conn.ExecuteScalarAsync<int>(countSql, new { Id = step.Id });
        Assert.Equal(0, after);
    }
    
    [Fact]
    public async Task TestGetLabelWithSteps()
    {
        var label = CreateLabel("label_with_steps");
        await labelRepo.AddOrUpdateFullAsync(label);
        
        var step1 = CreateStep(label, 1);
        var step2 = CreateStep(label, 2);
        var ctx = new LoadContext();
        
        await stepRepo.AddOrUpdateFullAsync(step1, ctx);
        await stepRepo.AddOrUpdateFullAsync(step2, ctx);
        
        var result = await labelRepo.GetFullByIdAsync(label.Id, new LoadContext());
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Steps.Count);
        Assert.Contains(result.Steps, s => s.Id == step1.Id);
        Assert.Contains(result.Steps, s => s.Id == step2.Id);
        
        Assert.Equal(1, result.Steps[0].StepOrder);
        Assert.Equal(step1.Id, result.Steps[0].Id);
        
        Assert.Equal(2, result.Steps[1].StepOrder);
        Assert.Equal(step2.Id, result.Steps[1].Id);
    }
    
    [Fact]
    public async Task TestCreateLabelWithSteps()
    {
        // Arrange
        var label = CreateLabel("label_with_new_steps");
        var step1 = CreateStep(label, 1);
        var step2 = CreateStep(label, 2);
        label.Steps = [step1, step2];
        
        var ctx = new LoadContext();
        await labelRepo.AddOrUpdateFullAsync(label, ctx);
        
        const string sql = "SELECT COUNT(*) FROM \"Steps\" WHERE label_id = @LabelId";
        await using var conn = new NpgsqlConnection(connectionString);
        var count = await conn.ExecuteScalarAsync<int>(sql, new { LabelId = label.Id });
        
        Assert.Equal(2, count);
    }
    
    [Fact]
    public async Task TestUpdateLabelWithSteps()
    {
        var label = CreateLabel("label_to_update");
        var step1 = CreateStep(label, 1);
        label.Steps = new List<StepDbO> { step1 };
        
        var ctx = new LoadContext();
        await labelRepo.AddOrUpdateFullAsync(label, ctx);
        
        var step2 = CreateStep(label, 2);
        step1.StepOrder = 999;
        label.Steps.Add(step2);
        label.LabelName = "updated_label_name";
        
        await labelRepo.AddOrUpdateFullAsync(label, new LoadContext());
        
        await using var conn = new NpgsqlConnection(connectionString);
        
        var stepCount = await conn.ExecuteScalarAsync<int>(
            @"SELECT COUNT(*) FROM ""Steps"" WHERE label_id = @LabelId",
            new { LabelId = label.Id });

        var dbName = await conn.ExecuteScalarAsync<string>(
            @"SELECT label_name FROM ""Labels"" WHERE id = @Id",
            new { Id = label.Id });

        var updatedStepOrder = await conn.ExecuteScalarAsync<int>(
            @"SELECT step_order FROM ""Steps"" WHERE id = @Id",
            new { Id = step1.Id });

        Assert.Equal(2, stepCount);
        Assert.Equal("updated_label_name", dbName);
        Assert.Equal(999, updatedStepOrder);
    }
    
    [Fact]
    public async Task TestDeleteLabelWithSteps()
    {
        var label1 = CreateLabel("novel_label_1");
        var label2 = CreateLabel("novel_label_2");
        
        await labelRepo.AddOrUpdateFullAsync(label1);
        await labelRepo.AddOrUpdateFullAsync(label2);
        
        var result = await labelRepo.GetFullByNovelIdAsync(novelId);
        var resultList = result.ToList();
        
        Assert.Equal(2, resultList.Count);
        Assert.Contains(resultList, l => l?.Id == label1.Id);
        Assert.Contains(resultList, l => l?.Id == label2.Id);
    }
    
    [Fact]
    public async Task TestUpdateLabelRemovesObsoleteSteps()
    {
        var label = CreateLabel("label_with_steps");
        var step1 = CreateStep(label, 1);
        var step2 = CreateStep(label, 2);
        var step3 = CreateStep(label, 3);
    
        label.Steps = [step1, step2, step3];
        await labelRepo.AddOrUpdateFullAsync(label);
    
        await using var conn = new NpgsqlConnection(connectionString);
        var beforeCount = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM \"Steps\" WHERE label_id = @LabelId", 
            new { LabelId = label.Id });
        Assert.Equal(3, beforeCount);
    
        label.Steps = [step1, step3];
        await labelRepo.AddOrUpdateFullAsync(label);
        
        var afterCount = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM \"Steps\" WHERE label_id = @LabelId", 
            new { LabelId = label.Id });
        Assert.Equal(2, afterCount);
    
        var step2Exists = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM \"Steps\" WHERE id = @Id", 
            new { Id = step2.Id });
        Assert.Equal(0, step2Exists);
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