using System.Net;
using NoviVovi.Api.Characters.Requests;
using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Images.Requests;
using NoviVovi.Api.Images.Responses;
using NoviVovi.Api.Labels.Requests;
using NoviVovi.Api.Labels.Responses;
using NoviVovi.Api.Novels.Requests;
using NoviVovi.Api.Novels.Responses;
using NoviVovi.Api.Scene.Requests;
using NoviVovi.Api.Steps.Requests;
using NoviVovi.Api.Steps.Responses;
using NoviVovi.Api.Tests.Infrastructure;

namespace NoviVovi.Api.Tests.Security;

/// <summary>
/// Tests to verify that entities from one novel cannot be accessed or modified through another novel's endpoints.
/// This is critical for multi-tenant security.
/// </summary>
[Collection("Database collection")]
public class SecurityTests(NoviVoviWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    #region Helper Methods

    private async Task<Guid> CreateNovelAsync(string title)
    {
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest(title));
        Assert.NotNull(novel);
        return novel.Id;
    }

    private async Task<Guid> CreateLabelAsync(Guid novelId, string name)
    {
        var label = await PostAsync<LabelResponse>($"/api/novels/{novelId}/labels", new AddLabelRequest(name));
        Assert.NotNull(label);
        return label.Id;
    }

    private async Task<Guid> CreateCharacterAsync(Guid novelId, string name)
    {
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novelId}/characters",
            new AddCharacterRequest(name, "FF5733", null));
        Assert.NotNull(character);
        return character.Id;
    }

    private async Task<Guid> CreateImageAsync(Guid novelId)
    {
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novelId}/images/upload-url",
            new InitiateUploadImageRequest("test.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        Assert.NotNull(uploadInfo);
        await Client.PostAsync($"/api/novels/{novelId}/images/{uploadInfo.ImageId}/confirm", null);
        return uploadInfo.ImageId;
    }

    private async Task<Guid> CreateCharacterStateAsync(Guid novelId, Guid characterId, Guid imageId)
    {
        var state = await PostAsync<CharacterStateResponse>(
            $"/api/novels/{novelId}/characters/{characterId}/states",
            new AddCharacterStateRequest("happy", null, imageId, new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        Assert.NotNull(state);
        return state.Id;
    }

    private async Task<Guid> CreateStepAsync(Guid novelId, Guid labelId, Guid characterId)
    {
        var step = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowReplicaStepRequest(characterId, "Test text"));
        Assert.NotNull(step);
        return step.Id;
    }

    #endregion

    #region Labels Security Tests

    [Fact]
    public async Task GetLabel_WrongNovelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var label1Id = await CreateLabelAsync(novel1Id, "label1");

        // Act - Try to access novel1's label through novel2's endpoint
        var response = await GetRawAsync($"/api/novels/{novel2Id}/labels/{label1Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PatchLabel_WrongNovelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var label1Id = await CreateLabelAsync(novel1Id, "label1");

        // Act - Try to update novel1's label through novel2's endpoint
        var response = await PatchRawAsync($"/api/novels/{novel2Id}/labels/{label1Id}",
            new PatchLabelRequest("hacked"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteLabel_WrongNovelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var label1Id = await CreateLabelAsync(novel1Id, "label1");

        // Act - Try to delete novel1's label through novel2's endpoint
        var response = await DeleteRawAsync($"/api/novels/{novel2Id}/labels/{label1Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        // Verify label still exists in novel1
        var label = await GetAsync<LabelResponse>($"/api/novels/{novel1Id}/labels/{label1Id}");
        Assert.NotNull(label);
    }

    #endregion

    #region Characters Security Tests

    [Fact]
    public async Task GetCharacter_WrongNovelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var char1Id = await CreateCharacterAsync(novel1Id, "Character1");

        // Act
        var response = await GetRawAsync($"/api/novels/{novel2Id}/characters/{char1Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PatchCharacter_WrongNovelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var char1Id = await CreateCharacterAsync(novel1Id, "Character1");

        // Act
        var response = await PatchRawAsync($"/api/novels/{novel2Id}/characters/{char1Id}",
            new PatchCharacterRequest("Hacked", null, null));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCharacter_WrongNovelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var char1Id = await CreateCharacterAsync(novel1Id, "Character1");

        // Act
        var response = await DeleteRawAsync($"/api/novels/{novel2Id}/characters/{char1Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        // Verify character still exists
        var character = await GetAsync<CharacterResponse>($"/api/novels/{novel1Id}/characters/{char1Id}");
        Assert.NotNull(character);
    }

    #endregion

    #region Character States Security Tests

    [Fact]
    public async Task GetCharacterState_WrongNovelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var char1Id = await CreateCharacterAsync(novel1Id, "Character1");
        var image1Id = await CreateImageAsync(novel1Id);
        var state1Id = await CreateCharacterStateAsync(novel1Id, char1Id, image1Id);

        // Act
        var response = await GetRawAsync($"/api/novels/{novel2Id}/characters/{char1Id}/states/{state1Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddCharacterState_CharacterFromDifferentNovel_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var char1Id = await CreateCharacterAsync(novel1Id, "Character1");
        var image2Id = await CreateImageAsync(novel2Id);

        // Act - Try to add state to novel1's character using novel2's image
        var response = await PostRawAsync($"/api/novels/{novel1Id}/characters/{char1Id}/states",
            new AddCharacterStateRequest("happy", null, image2Id, new TransformRequest(0, 0, 512, 512, 1, 0, 0)));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PatchCharacterState_WrongNovelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var char1Id = await CreateCharacterAsync(novel1Id, "Character1");
        var image1Id = await CreateImageAsync(novel1Id);
        var state1Id = await CreateCharacterStateAsync(novel1Id, char1Id, image1Id);

        // Act
        var response = await PatchRawAsync($"/api/novels/{novel2Id}/characters/{char1Id}/states/{state1Id}",
            new PatchCharacterStateRequest("hacked", null, null, null));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCharacterState_WrongNovelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var char1Id = await CreateCharacterAsync(novel1Id, "Character1");
        var image1Id = await CreateImageAsync(novel1Id);
        var state1Id = await CreateCharacterStateAsync(novel1Id, char1Id, image1Id);

        // Act
        var response = await DeleteRawAsync($"/api/novels/{novel2Id}/characters/{char1Id}/states/{state1Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        // Verify state still exists
        var state = await GetAsync<CharacterStateResponse>($"/api/novels/{novel1Id}/characters/{char1Id}/states/{state1Id}");
        Assert.NotNull(state);
    }

    #endregion

    #region Images Security Tests

    [Fact]
    public async Task GetImage_WrongNovelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var image1Id = await CreateImageAsync(novel1Id);

        // Act
        var response = await GetRawAsync($"/api/novels/{novel2Id}/images/{image1Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ConfirmImage_WrongNovelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel1Id}/images/upload-url",
            new InitiateUploadImageRequest("test.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        Assert.NotNull(uploadInfo);

        // Act - Try to confirm novel1's image through novel2's endpoint
        var response = await Client.PostAsync($"/api/novels/{novel2Id}/images/{uploadInfo.ImageId}/confirm", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PatchImage_WrongNovelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var image1Id = await CreateImageAsync(novel1Id);

        // Act
        var response = await PatchRawAsync($"/api/novels/{novel2Id}/images/{image1Id}",
            new PatchImageRequest("hacked.png", null));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteImage_WrongNovelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var image1Id = await CreateImageAsync(novel1Id);

        // Act
        var response = await DeleteRawAsync($"/api/novels/{novel2Id}/images/{image1Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        // Verify image still exists
        var image = await GetAsync<ImageResponse>($"/api/novels/{novel1Id}/images/{image1Id}");
        Assert.NotNull(image);
    }

    #endregion

    #region Steps Security Tests

    [Fact]
    public async Task GetStep_WrongNovelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var label1Id = await CreateLabelAsync(novel1Id, "label1");
        var char1Id = await CreateCharacterAsync(novel1Id, "Character1");
        var step1Id = await CreateStepAsync(novel1Id, label1Id, char1Id);

        // Act
        var response = await GetRawAsync($"/api/novels/{novel2Id}/labels/{label1Id}/steps/{step1Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetStep_WrongLabelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var label1Id = await CreateLabelAsync(novel1Id, "label1");
        var label2Id = await CreateLabelAsync(novel1Id, "label2");
        var char1Id = await CreateCharacterAsync(novel1Id, "Character1");
        var step1Id = await CreateStepAsync(novel1Id, label1Id, char1Id);

        // Act - Try to access label1's step through label2's endpoint
        var response = await GetRawAsync($"/api/novels/{novel1Id}/labels/{label2Id}/steps/{step1Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddStep_CharacterFromDifferentNovel_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var label1Id = await CreateLabelAsync(novel1Id, "label1");
        var char2Id = await CreateCharacterAsync(novel2Id, "Character2");

        // Act - Try to create step in novel1 using novel2's character
        var response = await PostRawAsync($"/api/novels/{novel1Id}/labels/{label1Id}/steps",
            new AddShowReplicaStepRequest(char2Id, "Test"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddJumpStep_TargetLabelFromDifferentNovel_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var label1Id = await CreateLabelAsync(novel1Id, "label1");
        var label2Id = await CreateLabelAsync(novel2Id, "label2");

        // Act - Try to create jump step in novel1 targeting novel2's label
        var response = await PostRawAsync($"/api/novels/{novel1Id}/labels/{label1Id}/steps",
            new AddJumpStepRequest(label2Id));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddShowBackgroundStep_ImageFromDifferentNovel_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var label1Id = await CreateLabelAsync(novel1Id, "label1");
        var image2Id = await CreateImageAsync(novel2Id);

        // Act - Try to create background step in novel1 using novel2's image
        var response = await PostRawAsync($"/api/novels/{novel1Id}/labels/{label1Id}/steps",
            new AddShowBackgroundStepRequest(image2Id, new TransformRequest(0, 0, 1920, 1080, 1, 0, 0)));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PatchStep_WrongNovelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var label1Id = await CreateLabelAsync(novel1Id, "label1");
        var char1Id = await CreateCharacterAsync(novel1Id, "Character1");
        var step1Id = await CreateStepAsync(novel1Id, label1Id, char1Id);

        // Act
        var response = await PatchRawAsync($"/api/novels/{novel2Id}/labels/{label1Id}/steps/{step1Id}",
            new PatchShowReplicaStepRequest(char1Id, "Hacked"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteStep_WrongNovelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var label1Id = await CreateLabelAsync(novel1Id, "label1");
        var char1Id = await CreateCharacterAsync(novel1Id, "Character1");
        var step1Id = await CreateStepAsync(novel1Id, label1Id, char1Id);

        // Act
        var response = await DeleteRawAsync($"/api/novels/{novel2Id}/labels/{label1Id}/steps/{step1Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        // Verify step still exists
        var step = await GetAsync<ShowReplicaStepResponse>($"/api/novels/{novel1Id}/labels/{label1Id}/steps/{step1Id}");
        Assert.NotNull(step);
    }

    #endregion

    #region Preview Security Tests

    [Fact]
    public async Task GetPreview_WrongNovelId_ReturnsNotFound()
    {
        // Arrange
        var novel1Id = await CreateNovelAsync("Novel 1");
        var novel2Id = await CreateNovelAsync("Novel 2");
        var label1Id = await CreateLabelAsync(novel1Id, "label1");
        var char1Id = await CreateCharacterAsync(novel1Id, "Character1");
        var step1Id = await CreateStepAsync(novel1Id, label1Id, char1Id);

        // Act
        var response = await GetRawAsync($"/preview/novels/{novel2Id}/labels/{label1Id}/steps/{step1Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion
}
