using System.Net;
using Dapper;
using NoviVovi.Api.Labels.Requests;
using NoviVovi.Api.Labels.Responses;
using NoviVovi.Api.Novels.Requests;
using NoviVovi.Api.Novels.Responses;
using NoviVovi.Api.Tests.Infrastructure;

namespace NoviVovi.Api.Tests.Labels;

[Collection("Database collection")]
public class LabelsControllerTests(NoviVoviWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private async Task<Guid> CreateTestNovelAsync()
    {
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test Novel"));
        Assert.NotNull(novel);
        return novel.Id;
    }

    [Fact]
    public async Task AddLabel_ValidRequest_ReturnsCreatedLabel()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var request = new AddLabelRequest("chapter1");

        // Act
        var response = await PostAsync<LabelResponse>($"/api/novels/{novelId}/labels", request);

        // Assert
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("chapter1", response.Name);

        // Verify in database
        var dbLabel = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Labels"" WHERE ""id"" = @Id",
            new { Id = response.Id });
        
        Assert.NotNull(dbLabel);
        Assert.Equal("chapter1", (string)dbLabel.label_name);
        Assert.Equal(novelId, (Guid)dbLabel.novel_id);
    }

    [Fact]
    public async Task AddLabel_EmptyName_ReturnsUnprocessableEntity()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var request = new AddLabelRequest("");

        // Act
        var response = await PostRawAsync($"/api/novels/{novelId}/labels", request);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task AddLabel_NonExistingNovel_ReturnsNotFound()
    {
        // Arrange
        var nonExistingNovelId = Guid.NewGuid();
        var request = new AddLabelRequest("chapter1");

        // Act
        var response = await PostRawAsync($"/api/novels/{nonExistingNovelId}/labels", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetLabel_ExistingId_ReturnsLabel()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var created = await PostAsync<LabelResponse>($"/api/novels/{novelId}/labels", 
            new AddLabelRequest("chapter1"));
        Assert.NotNull(created);

        // Act
        var response = await GetAsync<LabelResponse>($"/api/novels/{novelId}/labels/{created.Id}");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(created.Id, response.Id);
        Assert.Equal("chapter1", response.Name);
    }

    [Fact]
    public async Task GetLabel_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await GetRawAsync($"/api/novels/{novelId}/labels/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllLabels_ReturnsAllLabelsForNovel()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        
        // Novel already has start label, add more
        await PostAsync<LabelResponse>($"/api/novels/{novelId}/labels", new AddLabelRequest("chapter1"));
        await PostAsync<LabelResponse>($"/api/novels/{novelId}/labels", new AddLabelRequest("chapter2"));
        await PostAsync<LabelResponse>($"/api/novels/{novelId}/labels", new AddLabelRequest("chapter3"));

        // Act
        var response = await GetListAsync<LabelResponse>($"/api/novels/{novelId}/labels");

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Count >= 4); // start + 3 created
        Assert.Contains(response, l => l.Name == "chapter1");
        Assert.Contains(response, l => l.Name == "chapter2");
        Assert.Contains(response, l => l.Name == "chapter3");
    }

    [Fact]
    public async Task GetAllLabels_DifferentNovels_ReturnsOnlyNovelLabels()
    {
        // Arrange
        var novel1Id = await CreateTestNovelAsync();
        var novel2Id = await CreateTestNovelAsync();
        
        await PostAsync<LabelResponse>($"/api/novels/{novel1Id}/labels", new AddLabelRequest("novel1_label"));
        await PostAsync<LabelResponse>($"/api/novels/{novel2Id}/labels", new AddLabelRequest("novel2_label"));

        // Act
        var novel1Labels = await GetListAsync<LabelResponse>($"/api/novels/{novel1Id}/labels");
        var novel2Labels = await GetListAsync<LabelResponse>($"/api/novels/{novel2Id}/labels");

        // Assert
        Assert.NotNull(novel1Labels);
        Assert.NotNull(novel2Labels);
        Assert.Contains(novel1Labels, l => l.Name == "novel1_label");
        Assert.DoesNotContain(novel1Labels, l => l.Name == "novel2_label");
        Assert.Contains(novel2Labels, l => l.Name == "novel2_label");
        Assert.DoesNotContain(novel2Labels, l => l.Name == "novel1_label");
    }

    [Fact]
    public async Task PatchLabel_ValidRequest_UpdatesLabel()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var created = await PostAsync<LabelResponse>($"/api/novels/{novelId}/labels", 
            new AddLabelRequest("original_name"));
        Assert.NotNull(created);

        var patchRequest = new PatchLabelRequest("updated_name");

        // Act
        var response = await PatchAsync<LabelResponse>($"/api/novels/{novelId}/labels/{created.Id}", patchRequest);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(created.Id, response.Id);
        Assert.Equal("updated_name", response.Name);

        // Verify in database
        var dbLabel = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Labels"" WHERE ""id"" = @Id",
            new { Id = created.Id });
        
        Assert.NotNull(dbLabel);
        Assert.Equal("updated_name", (string)dbLabel.label_name);
    }

    [Fact]
    public async Task PatchLabel_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var nonExistingId = Guid.NewGuid();
        var patchRequest = new PatchLabelRequest("updated");

        // Act
        var response = await PatchRawAsync($"/api/novels/{novelId}/labels/{nonExistingId}", patchRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteLabel_ExistingId_DeletesLabel()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var created = await PostAsync<LabelResponse>($"/api/novels/{novelId}/labels", 
            new AddLabelRequest("to_delete"));
        Assert.NotNull(created);

        // Act
        await DeleteAsync($"/api/novels/{novelId}/labels/{created.Id}");

        // Assert - verify deleted
        var getResponse = await GetRawAsync($"/api/novels/{novelId}/labels/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        // Verify in database
        var dbLabel = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Labels"" WHERE ""id"" = @Id",
            new { Id = created.Id });
        
        Assert.Null(dbLabel);
    }

    [Fact]
    public async Task DeleteLabel_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await DeleteRawAsync($"/api/novels/{novelId}/labels/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteLabel_CascadeDeletesSteps()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var label = await PostAsync<LabelResponse>($"/api/novels/{novelId}/labels", 
            new AddLabelRequest("label_with_steps"));
        Assert.NotNull(label);

        // Create a step for this label
        var replicaId = Guid.NewGuid();
        var stepId = Guid.NewGuid();
        await UnitOfWork.Connection.ExecuteAsync(@"
            INSERT INTO ""Replicas"" (""id"", ""speaker_id"", ""text"")
            VALUES (@ReplicaId, NULL, 'Test text');
            
            INSERT INTO ""Steps"" (""id"", ""label_id"", ""replica_id"", ""step_order"", ""step_type"")
            VALUES (@StepId, @LabelId, @ReplicaId, 1, 'replica');
        ", new { StepId = stepId, LabelId = label.Id, ReplicaId = replicaId });

        // Act
        await DeleteAsync($"/api/novels/{novelId}/labels/{label.Id}");

        // Assert - verify steps are also deleted (CASCADE)
        var stepsCount = await QuerySingleAsync<int>(
            @"SELECT COUNT(*) FROM ""Steps"" WHERE ""label_id"" = @LabelId",
            new { LabelId = label.Id });
        
        Assert.Equal(0, stepsCount);
    }
}
