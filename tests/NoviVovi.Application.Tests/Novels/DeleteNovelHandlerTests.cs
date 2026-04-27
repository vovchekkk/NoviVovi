using Moq;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Novels.Features.Delete;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Tests.Novels;

public class DeleteNovelHandlerTests
{
    private readonly Mock<INovelRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly DeleteNovelHandler _handler;

    public DeleteNovelHandlerTests()
    {
        _mockRepository = new Mock<INovelRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteNovelHandler(_mockRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ExistingNovel_DeletesSuccessfully()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var novel = Novel.Create("Test Novel");

        _mockRepository
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

        _mockRepository
            .Setup(r => r.DeleteAsync(novel, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.BeginTransaction());
        _mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var command = new DeleteNovelCommand(novelId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(novel, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.BeginTransaction(), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingNovel_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Novel?)null);

        var command = new DeleteNovelCommand(novelId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        _mockRepository.Verify(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Novel>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
