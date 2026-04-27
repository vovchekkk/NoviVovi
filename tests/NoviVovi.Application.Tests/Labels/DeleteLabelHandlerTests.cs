using Moq;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Labels.Features.Delete;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Tests.Labels;

public class DeleteLabelHandlerTests
{
    private readonly Mock<INovelRepository> _mockNovelRepo;
    private readonly Mock<ILabelRepository> _mockLabelRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly DeleteLabelHandler _handler;

    public DeleteLabelHandlerTests()
    {
        _mockNovelRepo = new Mock<INovelRepository>();
        _mockLabelRepo = new Mock<ILabelRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteLabelHandler(_mockNovelRepo.Object, _mockLabelRepo.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ExistingLabel_DeletesSuccessfully()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var novel = Novel.Create("Test Novel");
        
        // Create label and add to novel so it exists in the novel's collection
        var label = Label.Create("chapter1", novelId);
        novel.AddLabel(label);
        var labelId = label.Id;

        _mockNovelRepo
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

        _mockLabelRepo
            .Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(label);

        _mockLabelRepo
            .Setup(r => r.DeleteAsync(label, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockNovelRepo
            .Setup(r => r.AddOrUpdateAsync(novel, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.BeginTransaction());
        _mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var command = new DeleteLabelCommand(novelId, labelId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockLabelRepo.Verify(r => r.DeleteAsync(label, It.IsAny<CancellationToken>()), Times.Once);
        _mockNovelRepo.Verify(r => r.AddOrUpdateAsync(novel, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingLabel_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var labelId = Guid.NewGuid();
        var novel = Novel.Create("Test Novel");

        _mockNovelRepo
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

        _mockLabelRepo
            .Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Label?)null);

        var command = new DeleteLabelCommand(novelId, labelId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        _mockLabelRepo.Verify(r => r.DeleteAsync(It.IsAny<Label>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
