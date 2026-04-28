using System.Net;
using Dapper;
using NoviVovi.Api.Images.Requests;
using NoviVovi.Api.Images.Responses;
using NoviVovi.Api.Novels.Requests;
using NoviVovi.Api.Novels.Responses;
using NoviVovi.Api.Scene.Requests;
using NoviVovi.Api.Tests.Infrastructure;

namespace NoviVovi.Api.Tests.Images;

[Collection("Database collection")]
public class ImagesControllerTests(NoviVoviWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private async Task<Guid> CreateTestNovelAsync()
    {
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test Novel"));
        Assert.NotNull(novel);
        return novel.Id;
    }

    [Fact]
    public async Task InitiateUpload_ValidRequest_ReturnsUploadInfo()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var request = new InitiateUploadImageRequest(
            "background.png",
            "Test background",
            "png",
            ImageTypeRequest.Background,
            new SizeRequest(1920, 1080)
        );

        // Act
        var response = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novelId}/images/upload-url", request);

        // Assert
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.ImageId);
        Assert.NotNull(response.UploadUrl);
        Assert.Contains("mock-storage.test", response.UploadUrl);

        // Verify image created in database
        var dbImage = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Images"" WHERE ""id"" = @Id",
            new { Id = response.ImageId });
        
        Assert.NotNull(dbImage);
        Assert.Equal("background.png", (string)dbImage.name);
        Assert.Equal("png", (string)dbImage.format);
    }

    [Fact]
    public async Task InitiateUpload_EmptyName_ReturnsUnprocessableEntity()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var request = new InitiateUploadImageRequest(
            "",
            null,
            "png",
            ImageTypeRequest.Background,
            new SizeRequest(1920, 1080)
        );

        // Act
        var response = await PostRawAsync($"/api/novels/{novelId}/images/upload-url", request);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task InitiateUpload_InvalidFormat_ReturnsUnprocessableEntity()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var request = new InitiateUploadImageRequest(
            "image.xyz",
            null,
            "xyz", // Invalid format
            ImageTypeRequest.Background,
            new SizeRequest(1920, 1080)
        );

        // Act
        var response = await PostRawAsync($"/api/novels/{novelId}/images/upload-url", request);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task ConfirmUpload_ValidImageId_ConfirmsUpload()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novelId}/images/upload-url",
            new InitiateUploadImageRequest(
                "test.png",
                null,
                "png",
                ImageTypeRequest.Character,
                new SizeRequest(512, 512)
            ));
        Assert.NotNull(uploadInfo);

        // Act
        var response = await Client.PostAsync($"/api/novels/{novelId}/images/{uploadInfo.ImageId}/confirm", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify image exists in database
        var dbImage = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Images"" WHERE ""id"" = @Id",
            new { Id = uploadInfo.ImageId });
        
        Assert.NotNull(dbImage);
    }

    [Fact]
    public async Task ConfirmUpload_NonExistingImageId_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await Client.PostAsync($"/api/novels/{novelId}/images/{nonExistingId}/confirm", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetImage_ExistingId_ReturnsImage()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novelId}/images/upload-url",
            new InitiateUploadImageRequest(
                "test.png",
                null,
                "png",
                ImageTypeRequest.Background,
                new SizeRequest(1920, 1080)
            ));
        Assert.NotNull(uploadInfo);

        await Client.PostAsync($"/api/novels/{novelId}/images/{uploadInfo.ImageId}/confirm", null);

        // Act
        var response = await GetAsync<ImageResponse>($"/api/novels/{novelId}/images/{uploadInfo.ImageId}");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(uploadInfo.ImageId, response.Id);
        Assert.Equal("test.png", response.Name);
        Assert.Equal("png", response.Format);
        Assert.Equal(ImageTypeResponse.Background, response.Type);
        Assert.NotNull(response.Size);
        Assert.Equal(1920, response.Size.Width);
        Assert.Equal(1080, response.Size.Height);
    }

    [Fact]
    public async Task GetImage_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await GetRawAsync($"/api/novels/{novelId}/images/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PatchImage_ValidRequest_UpdatesImage()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novelId}/images/upload-url",
            new InitiateUploadImageRequest(
                "original.png",
                "Original description",
                "png",
                ImageTypeRequest.Character,
                new SizeRequest(512, 512)
            ));
        Assert.NotNull(uploadInfo);

        await Client.PostAsync($"/api/novels/{novelId}/images/{uploadInfo.ImageId}/confirm", null);

        var patchRequest = new PatchImageRequest("updated.png", "Updated description", null);

        // Act
        var response = await PatchAsync<ImageResponse>($"/api/novels/{novelId}/images/{uploadInfo.ImageId}", patchRequest);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(uploadInfo.ImageId, response.Id);
        Assert.Equal("updated.png", response.Name);
        Assert.Equal("Updated description", response.Description);

        // Verify in database
        var dbImage = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Images"" WHERE ""id"" = @Id",
            new { Id = uploadInfo.ImageId });
        
        Assert.NotNull(dbImage);
        Assert.Equal("updated.png", (string)dbImage.name);
    }

    [Fact]
    public async Task PatchImage_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var nonExistingId = Guid.NewGuid();
        var patchRequest = new PatchImageRequest("updated.png", null, null);

        // Act
        var response = await PatchRawAsync($"/api/novels/{novelId}/images/{nonExistingId}", patchRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteImage_ExistingId_DeletesImage()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novelId}/images/upload-url",
            new InitiateUploadImageRequest(
                "to_delete.png",
                null,
                "png",
                ImageTypeRequest.Background,
                new SizeRequest(1920, 1080)
            ));
        Assert.NotNull(uploadInfo);

        await Client.PostAsync($"/api/novels/{novelId}/images/{uploadInfo.ImageId}/confirm", null);

        // Act
        await DeleteAsync($"/api/novels/{novelId}/images/{uploadInfo.ImageId}");

        // Assert - verify deleted
        var getResponse = await GetRawAsync($"/api/novels/{novelId}/images/{uploadInfo.ImageId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        // Verify in database
        var dbImage = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Images"" WHERE ""id"" = @Id",
            new { Id = uploadInfo.ImageId });
        
        Assert.Null(dbImage);
    }

    [Fact]
    public async Task DeleteImage_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await DeleteRawAsync($"/api/novels/{novelId}/images/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ImageWorkflow_CompleteFlow_WorksCorrectly()
    {
        // This test verifies the complete image upload workflow

        // Step 1: Initiate upload
        var novelId = await CreateTestNovelAsync();
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novelId}/images/upload-url",
            new InitiateUploadImageRequest(
                "workflow_test.png",
                "Testing complete workflow",
                "png",
                ImageTypeRequest.Character,
                new SizeRequest(512, 512)
            ));
        Assert.NotNull(uploadInfo);
        Assert.NotNull(uploadInfo.UploadUrl);

        // Step 2: Simulate file upload to presigned URL (in real scenario, client uploads file here)
        // Our mock storage service always returns success

        // Step 3: Confirm upload
        var confirmResponse = await Client.PostAsync($"/api/novels/{novelId}/images/{uploadInfo.ImageId}/confirm", null);
        Assert.Equal(HttpStatusCode.OK, confirmResponse.StatusCode);

        // Step 4: Get image details
        var image = await GetAsync<ImageResponse>($"/api/novels/{novelId}/images/{uploadInfo.ImageId}");
        Assert.NotNull(image);
        Assert.Equal("workflow_test.png", image.Name);
        Assert.Equal("Testing complete workflow", image.Description);

        // Step 5: Update metadata
        var updated = await PatchAsync<ImageResponse>($"/api/novels/{novelId}/images/{uploadInfo.ImageId}",
            new PatchImageRequest("workflow_test_updated.png", null, null));
        Assert.NotNull(updated);
        Assert.Equal("workflow_test_updated.png", updated.Name);

        // Step 6: Delete image
        await DeleteAsync($"/api/novels/{novelId}/images/{uploadInfo.ImageId}");
        var deletedCheck = await GetRawAsync($"/api/novels/{novelId}/images/{uploadInfo.ImageId}");
        Assert.Equal(HttpStatusCode.NotFound, deletedCheck.StatusCode);
    }
}
