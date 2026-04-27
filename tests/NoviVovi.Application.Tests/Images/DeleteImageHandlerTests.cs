using Moq;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Images.Abstractions;
using NoviVovi.Application.Images.Features.Delete;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Scene;

namespace NoviVovi.Application.Tests.Images;

public class DeleteImageHandlerTests
{
    private readonly Mock<IImageRepository> _mockImageRepo;
    private readonly Mock<IStorageService> _mockStorageService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly DeleteImageHandler _handler;

    public DeleteImageHandlerTests()
    {
        _mockImageRepo = new Mock<IImageRepository>();
        _mockStorageService = new Mock<IStorageService>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteImageHandler(_mockImageRepo.Object, _mockStorageService.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ExistingImage_DeletesSuccessfully()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var novelId = Guid.NewGuid();
        var image = Image.CreatePending("test.png", novelId, "path/test.png", "png", ImageType.Background, new Size(1920, 1080));

        _mockImageRepo
            .Setup(r => r.GetByIdAsync(imageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(image);

        _mockImageRepo
            .Setup(r => r.DeleteAsync(image, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockStorageService
            .Setup(s => s.DeleteFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.BeginTransaction());
        _mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var command = new DeleteImageCommand(imageId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockImageRepo.Verify(r => r.GetByIdAsync(imageId, It.IsAny<CancellationToken>()), Times.Once);
        _mockImageRepo.Verify(r => r.DeleteAsync(image, It.IsAny<CancellationToken>()), Times.Once);
        _mockStorageService.Verify(s => s.DeleteFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingImage_ThrowsNotFoundException()
    {
        // Arrange
        var imageId = Guid.NewGuid();

        _mockImageRepo
            .Setup(r => r.GetByIdAsync(imageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Image?)null);

        var command = new DeleteImageCommand(imageId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        _mockImageRepo.Verify(r => r.DeleteAsync(It.IsAny<Image>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
