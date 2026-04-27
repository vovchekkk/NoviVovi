using Moq;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Images.Abstractions;
using NoviVovi.Application.Images.Dtos;
using NoviVovi.Application.Images.Features.InitiateUpload;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Images.Models;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Domain.Images;

namespace NoviVovi.Application.Tests.Images;

public class InitiateUploadImageHandlerTests
{
    private readonly Mock<IImageRepository> _mockImageRepo;
    private readonly Mock<IStorageService> _mockStorageService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly UploadInfoImageDtoMapper _mockMapper;
    private readonly InitiateUploadImageHandler _handler;

    public InitiateUploadImageHandlerTests()
    {
        _mockImageRepo = new Mock<IImageRepository>();
        _mockStorageService = new Mock<IStorageService>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new UploadInfoImageDtoMapper();
        
        _handler = new InitiateUploadImageHandler(
            _mockImageRepo.Object,
            _mockStorageService.Object,
            _mockUnitOfWork.Object,
            _mockMapper
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUploadInfo()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var command = new InitiateUploadImageCommand 
        { 
            NovelId = novelId,
            Name = "test.png",
            Format = "png",
            Type = ImageType.Background,
            Size = new SizeDto(1920, 1080)
        };
        
        var expectedDto = new UploadInfoImageDto(
            Guid.NewGuid(),
            "https://storage.test/upload",
            "https://storage.test/view"
        );

        _mockStorageService
            .Setup(s => s.GetPresignedUploadUrlAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://storage.test/upload");

        _mockStorageService
            .Setup(s => s.GetViewUrl(It.IsAny<string>()))
            .Returns("https://storage.test/view");

        _mockImageRepo
            .Setup(r => r.AddOrUpdateAsync(It.IsAny<Image>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);


        _mockUnitOfWork.Setup(u => u.BeginTransaction());
        _mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("storage.test", result.UploadUrl);
        
        _mockImageRepo.Verify(r => r.AddOrUpdateAsync(It.IsAny<Image>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockStorageService.Verify(s => s.GetPresignedUploadUrlAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_StorageServiceThrowsException_RollsBack()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var command = new InitiateUploadImageCommand 
        { 
            NovelId = novelId,
            Name = "test.png",
            Format = "png",
            Type = ImageType.Background,
            Size = new SizeDto(1920, 1080)
        };

        _mockStorageService
            .Setup(s => s.GetPresignedUploadUrlAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Storage error"));

        _mockUnitOfWork.Setup(u => u.BeginTransaction());
        _mockUnitOfWork.Setup(u => u.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        _mockUnitOfWork.Verify(u => u.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
