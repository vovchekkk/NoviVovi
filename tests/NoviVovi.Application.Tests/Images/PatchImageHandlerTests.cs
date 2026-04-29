using Moq;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Images.Abstractions;
using NoviVovi.Application.Images.Dtos;
using NoviVovi.Application.Images.Features.Patch;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Scene;

namespace NoviVovi.Application.Tests.Images;

public class PatchImageHandlerTests
{
    private readonly Mock<IImageRepository> _mockImageRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IStorageService> _mockStorageService;
    private readonly ImageDtoMapper _mockMapper;
    private readonly PatchImageHandler _handler;

    public PatchImageHandlerTests()
    {
        _mockImageRepo = new Mock<IImageRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockStorageService = new Mock<IStorageService>();
        
        // ImageDtoMapper requires IStorageService and SizeDtoMapper
        _mockStorageService.Setup(s => s.GetViewUrl(It.IsAny<string>())).Returns("https://test.com/view");
        var sizeMapper = new SizeDtoMapper();
        _mockMapper = new ImageDtoMapper(_mockStorageService.Object, sizeMapper);
        
        _handler = new PatchImageHandler(_mockImageRepo.Object, _mockUnitOfWork.Object, _mockMapper);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesImage()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var novelId = Guid.NewGuid();
        var image = Image.CreatePending("test.png", novelId, "path/test.png", "png", ImageType.Background, new Size(1920, 1080));
        
        var command = new PatchImageCommand 
        { 
            ImageId = imageId,
            Name = "updated.png"
        };
        
        var expectedDto = new ImageDto
        {
            Id = imageId,
            Name = "updated.png",
            Format = "png",
            Type = ImageTypeDto.Background,
            Size = new SizeDto(1920, 1080),
            Url = "https://test.com/view",
            StoragePath = "path/test.png",
            Status = ImageStatusDto.Active
        };

        _mockImageRepo
            .Setup(r => r.GetByIdAsync(imageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(image);

        _mockImageRepo
            .Setup(r => r.AddOrUpdateAsync(image, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);


        _mockUnitOfWork.Setup(u => u.BeginTransaction());
        _mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("updated.png", result.Name);
        _mockImageRepo.Verify(r => r.AddOrUpdateAsync(image, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingImage_ThrowsNotFoundException()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        
        var command = new PatchImageCommand 
        { 
            ImageId = imageId,
            Name = "updated.png"
        };

        _mockImageRepo
            .Setup(r => r.GetByIdAsync(imageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Image?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        _mockImageRepo.Verify(r => r.AddOrUpdateAsync(It.IsAny<Image>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
