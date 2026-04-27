using Moq;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Images.Abstractions;
using NoviVovi.Application.Images.Dtos;
using NoviVovi.Application.Images.Features.Get;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Scene;

namespace NoviVovi.Application.Tests.Images;

public class GetImageHandlerTests
{
    private readonly Mock<IImageRepository> _mockImageRepo;
    private readonly Mock<IStorageService> _mockStorageService;
    private readonly Mock<ImageDtoMapper> _mockMapper;
    private readonly GetImageHandler _handler;

    public GetImageHandlerTests()
    {
        _mockImageRepo = new Mock<IImageRepository>();
        _mockStorageService = new Mock<IStorageService>();
        _mockMapper = new Mock<ImageDtoMapper>();
        _handler = new GetImageHandler(_mockImageRepo.Object, _mockStorageService.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ExistingImage_ReturnsDto()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var novelId = Guid.NewGuid();
        var image = Image.CreatePending("test.png", novelId, "path/test.png", "png", ImageType.Background, new Size(1920, 1080));
        
        var expectedDto = new ImageDto
        {
            Id = imageId,
            Name = "test.png",
            Description = null,
            Format = "png",
            Type = ImageTypeDto.Background,
            Size = new SizeDto(1920, 1080),
            Url = "https://test.com/view",
            StoragePath = "path/test.png",
            Status = ImageStatusDto.Pending
        };

        _mockImageRepo
            .Setup(r => r.GetByIdAsync(imageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(image);

        _mockMapper
            .Setup(m => m.ToDto(image))
            .Returns(expectedDto);

        var query = new GetImageQuery(imageId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test.png", result.Name);
        _mockImageRepo.Verify(r => r.GetByIdAsync(imageId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingImage_ThrowsNotFoundException()
    {
        // Arrange
        var imageId = Guid.NewGuid();

        _mockImageRepo
            .Setup(r => r.GetByIdAsync(imageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Image?)null);

        var query = new GetImageQuery(imageId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(query, CancellationToken.None));
    }
}
