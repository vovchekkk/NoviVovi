using System.Net;
using Dapper;
using NoviVovi.Api.Characters.Requests;
using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Novels.Requests;
using NoviVovi.Api.Novels.Responses;
using NoviVovi.Api.Tests.Infrastructure;

namespace NoviVovi.Api.Tests.Characters;

[Collection("Database collection")]
public class CharactersControllerTests(NoviVoviWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private async Task<Guid> CreateTestNovelAsync()
    {
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test Novel"));
        Assert.NotNull(novel);
        return novel.Id;
    }

    [Fact]
    public async Task AddCharacter_ValidRequest_ReturnsCreatedCharacter()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var request = new AddCharacterRequest("Alice", "FF5733", "Main character");

        // Act
        var response = await PostAsync<CharacterResponse>($"/api/novels/{novelId}/characters", request);

        // Assert
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("Alice", response.Name);
        Assert.Equal("#FF5733", response.NameColor); // API returns color with #
        Assert.Equal("Main character", response.Description);

        // Verify in database
        var dbCharacter = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Characters"" WHERE ""id"" = @Id",
            new { Id = response.Id });
        
        Assert.NotNull(dbCharacter);
        Assert.Equal("Alice", (string)dbCharacter.name);
        Assert.Equal("FF5733", (string)dbCharacter.name_color);
        Assert.Equal(novelId, (Guid)dbCharacter.novel_id);
    }

    [Fact]
    public async Task AddCharacter_EmptyName_ReturnsUnprocessableEntity()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var request = new AddCharacterRequest("", "FF5733", null);

        // Act
        var response = await PostRawAsync($"/api/novels/{novelId}/characters", request);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task AddCharacter_InvalidColorFormat_ReturnsUnprocessableEntity()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var request = new AddCharacterRequest("Alice", "GGGGGG", null); // Invalid hex

        // Act
        var response = await PostRawAsync($"/api/novels/{novelId}/characters", request);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task AddCharacter_NonExistingNovel_ReturnsNotFound()
    {
        // Arrange
        var nonExistingNovelId = Guid.NewGuid();
        var request = new AddCharacterRequest("Alice", "FF5733", null);

        // Act
        var response = await PostRawAsync($"/api/novels/{nonExistingNovelId}/characters", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCharacter_ExistingId_ReturnsCharacter()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var created = await PostAsync<CharacterResponse>($"/api/novels/{novelId}/characters",
            new AddCharacterRequest("Alice", "FF5733", null));
        Assert.NotNull(created);

        // Act
        var response = await GetAsync<CharacterResponse>($"/api/novels/{novelId}/characters/{created.Id}");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(created.Id, response.Id);
        Assert.Equal("Alice", response.Name);
        Assert.Equal("#FF5733", response.NameColor);
    }

    [Fact]
    public async Task GetCharacter_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await GetRawAsync($"/api/novels/{novelId}/characters/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllCharacters_ReturnsAllCharactersForNovel()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        
        await PostAsync<CharacterResponse>($"/api/novels/{novelId}/characters",
            new AddCharacterRequest("Alice", "FF5733", null));
        await PostAsync<CharacterResponse>($"/api/novels/{novelId}/characters",
            new AddCharacterRequest("Bob", "00FF00", null));
        await PostAsync<CharacterResponse>($"/api/novels/{novelId}/characters",
            new AddCharacterRequest("Charlie", "0000FF", null));

        // Act
        var response = await GetListAsync<CharacterResponse>($"/api/novels/{novelId}/characters");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(3, response.Count);
        Assert.Contains(response, c => c.Name == "Alice");
        Assert.Contains(response, c => c.Name == "Bob");
        Assert.Contains(response, c => c.Name == "Charlie");
    }

    [Fact]
    public async Task GetAllCharacters_DifferentNovels_ReturnsOnlyNovelCharacters()
    {
        // Arrange
        var novel1Id = await CreateTestNovelAsync();
        var novel2Id = await CreateTestNovelAsync();
        
        await PostAsync<CharacterResponse>($"/api/novels/{novel1Id}/characters",
            new AddCharacterRequest("Novel1Character", "FF5733", null));
        await PostAsync<CharacterResponse>($"/api/novels/{novel2Id}/characters",
            new AddCharacterRequest("Novel2Character", "00FF00", null));

        // Act
        var novel1Characters = await GetListAsync<CharacterResponse>($"/api/novels/{novel1Id}/characters");
        var novel2Characters = await GetListAsync<CharacterResponse>($"/api/novels/{novel2Id}/characters");

        // Assert
        Assert.NotNull(novel1Characters);
        Assert.NotNull(novel2Characters);
        Assert.Contains(novel1Characters, c => c.Name == "Novel1Character");
        Assert.DoesNotContain(novel1Characters, c => c.Name == "Novel2Character");
        Assert.Contains(novel2Characters, c => c.Name == "Novel2Character");
        Assert.DoesNotContain(novel2Characters, c => c.Name == "Novel1Character");
    }

    [Fact]
    public async Task PatchCharacter_ValidRequest_UpdatesCharacter()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var created = await PostAsync<CharacterResponse>($"/api/novels/{novelId}/characters",
            new AddCharacterRequest("Alice", "FF5733", "Original"));
        Assert.NotNull(created);

        var patchRequest = new PatchCharacterRequest("Alice Updated", "00FF00", "Updated description");

        // Act
        var response = await PatchAsync<CharacterResponse>($"/api/novels/{novelId}/characters/{created.Id}", patchRequest);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(created.Id, response.Id);
        Assert.Equal("Alice Updated", response.Name);
        Assert.Equal("#00FF00", response.NameColor);
        Assert.Equal("Updated description", response.Description);

        // Verify in database
        var dbCharacter = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Characters"" WHERE ""id"" = @Id",
            new { Id = created.Id });
        
        Assert.NotNull(dbCharacter);
        Assert.Equal("Alice Updated", (string)dbCharacter.name);
        Assert.Equal("00FF00", (string)dbCharacter.name_color);
    }

    [Fact]
    public async Task PatchCharacter_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var nonExistingId = Guid.NewGuid();
        var patchRequest = new PatchCharacterRequest("Updated", "FF5733", null);

        // Act
        var response = await PatchRawAsync($"/api/novels/{novelId}/characters/{nonExistingId}", patchRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCharacter_ExistingId_DeletesCharacter()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var created = await PostAsync<CharacterResponse>($"/api/novels/{novelId}/characters",
            new AddCharacterRequest("ToDelete", "FF5733", null));
        Assert.NotNull(created);

        // Act
        var deleteResponse = await DeleteRawAsync($"/api/novels/{novelId}/characters/{created.Id}");

        // Assert - DELETE should return NoContent (204)
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        
        // Verify character is deleted - GET should return NotFound
        var getResponse = await GetRawAsync($"/api/novels/{novelId}/characters/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        // Verify in database
        var dbCharacter = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Characters"" WHERE ""id"" = @Id",
            new { Id = created.Id });
        
        Assert.Null(dbCharacter);
    }

    [Fact]
    public async Task DeleteCharacter_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await DeleteRawAsync($"/api/novels/{novelId}/characters/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCharacter_CascadeDeletesCharacterStates()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novelId}/characters",
            new AddCharacterRequest("CharacterWithStates", "FF5733", null));
        Assert.NotNull(character);

        // Create a character state
        var imageId = Guid.NewGuid();
        var stateId = Guid.NewGuid();
        await UnitOfWork.Connection.ExecuteAsync(@"
            INSERT INTO ""Images"" (""id"", ""novel_id"", ""name"", ""url"", ""format"", ""img_type"", ""height"", ""width"", ""size"")
            VALUES (@ImageId, @NovelId, 'test.png', 'http://test.com/test.png', 'png', 'character', 512, 512, 1024);
            
            INSERT INTO ""CharacterStates"" (""id"", ""character_id"", ""image_id"", ""state_name"")
            VALUES (@StateId, @CharacterId, @ImageId, 'happy');
        ", new { StateId = stateId, CharacterId = character.Id, ImageId = imageId, NovelId = novelId });

        // Act
        await DeleteAsync($"/api/novels/{novelId}/characters/{character.Id}");

        // Assert - verify character states are also deleted (CASCADE)
        var statesCount = await QuerySingleAsync<int>(
            @"SELECT COUNT(*) FROM ""CharacterStates"" WHERE ""character_id"" = @CharacterId",
            new { CharacterId = character.Id });
        
        Assert.Equal(0, statesCount);
    }
}
