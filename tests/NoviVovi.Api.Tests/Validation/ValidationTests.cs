using System.Net;
using NoviVovi.Api.Characters.Requests;
using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Images.Requests;
using NoviVovi.Api.Images.Responses;
using NoviVovi.Api.Labels.Requests;
using NoviVovi.Api.Labels.Responses;
using NoviVovi.Api.Menu.Requests;
using NoviVovi.Api.Novels.Requests;
using NoviVovi.Api.Novels.Responses;
using NoviVovi.Api.Scene.Requests;
using NoviVovi.Api.Steps.Requests;
using NoviVovi.Api.Tests.Infrastructure;
using NoviVovi.Api.Transitions.Requests;

namespace NoviVovi.Api.Tests.Validation;

/// <summary>
/// Tests to verify input validation for all endpoints.
/// Ensures proper error handling for null, empty, and invalid values.
/// </summary>
[Collection("Database collection")]
public class ValidationTests(NoviVoviWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    #region Novels Validation

    [Fact]
    public async Task CreateNovel_EmptyTitle_Returns422()
    {
        // Act
        var response = await PostRawAsync("/api/novels", new CreateNovelRequest(""));

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task CreateNovel_WhitespaceTitle_Returns422()
    {
        // Act
        var response = await PostRawAsync("/api/novels", new CreateNovelRequest("   "));

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task PatchNovel_EmptyTitle_Returns422()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);

        // Act
        var response = await PatchRawAsync($"/api/novels/{novel.Id}", new PatchNovelRequest("", null));

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task GetNovel_InvalidGuid_Returns404()
    {
        // Act
        var response = await GetRawAsync($"/api/novels/{Guid.Empty}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Labels Validation

    [Fact]
    public async Task AddLabel_EmptyName_Returns422()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);

        // Act
        var response = await PostRawAsync($"/api/novels/{novel.Id}/labels", new AddLabelRequest(""));

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task AddLabel_WhitespaceName_Returns422()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);

        // Act
        var response = await PostRawAsync($"/api/novels/{novel.Id}/labels", new AddLabelRequest("   "));

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task GetLabel_NonExistentNovel_Returns404()
    {
        // Act
        var response = await GetRawAsync($"/api/novels/{Guid.NewGuid()}/labels/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Characters Validation

    [Fact]
    public async Task AddCharacter_EmptyName_Returns422()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);

        // Act
        var response = await PostRawAsync($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("", "FF5733", null));

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task AddCharacter_InvalidColor_Returns422()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);

        // Act - Invalid hex color
        var response = await PostRawAsync($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Character", "ZZZZZZ", null));

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task AddCharacter_ShortColor_Returns422()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);

        // Act - Color too short
        var response = await PostRawAsync($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Character", "FFF", null));

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    #endregion

    #region Images Validation

    [Fact]
    public async Task InitiateUploadImage_EmptyName_Returns422()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);

        // Act
        var response = await PostRawAsync($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task InitiateUploadImage_InvalidFormat_Returns422()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);

        // Act
        var response = await PostRawAsync($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("test.exe", "exe", ImageTypeRequest.Character, new SizeRequest(512, 512)));

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task InitiateUploadImage_NegativeSize_Returns422()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);

        // Act
        var response = await PostRawAsync($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("test.png", "png", ImageTypeRequest.Character, new SizeRequest(-100, 512)));

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task InitiateUploadImage_ZeroSize_Returns422()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);

        // Act
        var response = await PostRawAsync($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("test.png", "png", ImageTypeRequest.Character, new SizeRequest(0, 0)));

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    #endregion

    #region Steps Validation

    [Fact]
    public async Task AddReplicaStep_EmptyText_Returns422()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        var label = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("label1"));
        Assert.NotNull(label);
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Character", "FF5733", null));
        Assert.NotNull(character);

        // Act
        var response = await PostRawAsync($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowReplicaStepRequest(character.Id, ""));

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task AddReplicaStep_NonExistentCharacter_Returns404()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        var label = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("label1"));
        Assert.NotNull(label);

        // Act
        var response = await PostRawAsync($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowReplicaStepRequest(Guid.NewGuid(), "Test text"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddJumpStep_NonExistentLabel_Returns404()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        var label = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("label1"));
        Assert.NotNull(label);

        // Act
        var response = await PostRawAsync($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddJumpStepRequest(Guid.NewGuid()));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddMenuStep_EmptyChoices_Returns422()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        var label = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("label1"));
        Assert.NotNull(label);

        // Act
        var response = await PostRawAsync($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowMenuStepRequest([]));

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task AddMenuStep_ChoiceWithEmptyText_Returns422()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        var label = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("label1"));
        Assert.NotNull(label);

        // Act
        var response = await PostRawAsync($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowMenuStepRequest([
                new ChoiceRequest("", new ChoiceTransitionRequest { TargetLabelId = label.Id })
            ]));

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task AddShowBackgroundStep_NonExistentImage_Returns404()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        var label = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("label1"));
        Assert.NotNull(label);

        // Act
        var response = await PostRawAsync($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowBackgroundStepRequest(Guid.NewGuid(), new TransformRequest(0, 0, 1920, 1080, 1, 0, 0)));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddShowCharacterStep_NonExistentCharacterState_Returns404()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        var label = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("label1"));
        Assert.NotNull(label);

        // Act
        var response = await PostRawAsync($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowCharacterStepRequest(Guid.NewGuid(), new TransformRequest(0, 0, 512, 512, 1, 0, 0)));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Transform Validation

    [Fact]
    public async Task AddCharacterState_NegativeTransformValues_Returns422()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Character", "FF5733", null));
        Assert.NotNull(character);
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("test.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        Assert.NotNull(uploadInfo);
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{uploadInfo.ImageId}/confirm", null);

        // Act - Negative width/height
        var response = await PostRawAsync($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("happy", null, uploadInfo.ImageId, 
                new TransformRequest(0, 0, -100, -100, 1, 0, 0)));

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    #endregion

    #region Character States Validation

    [Fact]
    public async Task AddCharacterState_EmptyName_Returns422()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Character", "FF5733", null));
        Assert.NotNull(character);
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("test.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        Assert.NotNull(uploadInfo);
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{uploadInfo.ImageId}/confirm", null);

        // Act
        var response = await PostRawAsync($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("", null, uploadInfo.ImageId, 
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)));

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task AddCharacterState_NonExistentImage_Returns404()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Character", "FF5733", null));
        Assert.NotNull(character);

        // Act
        var response = await PostRawAsync($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("happy", null, Guid.NewGuid(), 
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion
}
