using System.Net;
using System.Net.Http.Json;
using NoviVovi.Api.Novels.Requests;
using NoviVovi.Api.Novels.Responses;
using NoviVovi.Api.Tests.Infrastructure;

namespace NoviVovi.Api.Tests.Novels;

[Collection("Database collection")]
public class NovelsControllerTests(NoviVoviWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task CreateNovel_ValidRequest_ReturnsCreatedNovel()
    {
        // Arrange
        var request = new CreateNovelRequest("Test Novel");

        // Act
        var response = await PostAsync<NovelResponse>("/api/novels", request);

        // Assert
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("Test Novel", response.Title);
        Assert.NotEqual(Guid.Empty, response.StartLabelId);

        // Verify in database
        var dbNovel = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Novels"" WHERE ""id"" = @Id",
            new { Id = response.Id });
        
        Assert.NotNull(dbNovel);
        Assert.Equal("Test Novel", (string)dbNovel.title);
    }

    [Fact]
    public async Task CreateNovel_EmptyTitle_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateNovelRequest("");

        // Act
        var response = await PostRawAsync("/api/novels", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetNovel_ExistingId_ReturnsNovel()
    {
        // Arrange
        var createRequest = new CreateNovelRequest("Test Novel");
        var created = await PostAsync<NovelResponse>("/api/novels", createRequest);
        Assert.NotNull(created);

        // Act
        var response = await GetAsync<NovelResponse>($"/api/novels/{created.Id}");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(created.Id, response.Id);
        Assert.Equal("Test Novel", response.Title);
    }

    [Fact]
    public async Task GetNovel_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await GetRawAsync($"/api/novels/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllNovels_ReturnsAllNovels()
    {
        // Arrange
        await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Novel 1"));
        await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Novel 2"));
        await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Novel 3"));

        // Act
        var response = await GetListAsync<NovelResponse>("/api/novels");

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Count >= 3);
        Assert.Contains(response, n => n.Title == "Novel 1");
        Assert.Contains(response, n => n.Title == "Novel 2");
        Assert.Contains(response, n => n.Title == "Novel 3");
    }

    [Fact]
    public async Task PatchNovel_ValidRequest_UpdatesNovel()
    {
        // Arrange
        var createRequest = new CreateNovelRequest("Original Title");
        var created = await PostAsync<NovelResponse>("/api/novels", createRequest);
        Assert.NotNull(created);

        var patchRequest = new PatchNovelRequest("Updated Title");

        // Act
        var response = await PatchAsync<NovelResponse>($"/api/novels/{created.Id}", patchRequest);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(created.Id, response.Id);
        Assert.Equal("Updated Title", response.Title);

        // Verify in database
        var dbNovel = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Novels"" WHERE ""id"" = @Id",
            new { Id = created.Id });
        
        Assert.NotNull(dbNovel);
        Assert.Equal("Updated Title", (string)dbNovel.title);
    }

    [Fact]
    public async Task DeleteNovel_ExistingId_DeletesNovel()
    {
        // Arrange
        var createRequest = new CreateNovelRequest("To Delete");
        var created = await PostAsync<NovelResponse>("/api/novels", createRequest);
        Assert.NotNull(created);

        // Act
        await DeleteAsync($"/api/novels/{created.Id}");

        // Assert - verify deleted
        var getResponse = await GetRawAsync($"/api/novels/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        // Verify in database
        var dbNovel = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Novels"" WHERE ""id"" = @Id",
            new { Id = created.Id });
        
        Assert.Null(dbNovel);
    }

    [Fact]
    public async Task DeleteNovel_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await DeleteRawAsync($"/api/novels/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetNovelGraph_ExistingNovel_ReturnsGraph()
    {
        // Arrange
        var createRequest = new CreateNovelRequest("Graph Test");
        var created = await PostAsync<NovelResponse>("/api/novels", createRequest);
        Assert.NotNull(created);

        // Act
        var response = await GetAsync<NovelGraphResponse>($"/api/novels/{created.Id}/graph");

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Nodes);
        Assert.NotNull(response.Edges);
        // Should have at least start label node
        Assert.NotEmpty(response.Nodes);
    }

    [Fact]
    public async Task GetNovelGraph_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await GetRawAsync($"/api/novels/{nonExistingId}/graph");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ExportNovelToRenPy_ExistingNovel_ReturnsZipFile()
    {
        // Arrange
        var createRequest = new CreateNovelRequest("Export Test");
        var created = await PostAsync<NovelResponse>("/api/novels", createRequest);
        Assert.NotNull(created);

        // Act
        var response = await Client.GetAsync($"/api/novels/{created.Id}/export/renpy");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/zip", response.Content.Headers.ContentType?.MediaType);
        
        var content = await response.Content.ReadAsByteArrayAsync();
        Assert.NotEmpty(content);
        
        // Verify ZIP signature (PK)
        Assert.Equal(0x50, content[0]); // 'P'
        Assert.Equal(0x4B, content[1]); // 'K'
    }

    [Fact]
    public async Task ExportNovelToRenPy_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await GetRawAsync($"/api/novels/{nonExistingId}/export/renpy");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
