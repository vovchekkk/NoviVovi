using Moq;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Features.Patch;
using NoviVovi.Application.Novels.Mappers;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Tests.Novels;

public class PatchNovelHandlerTests
{
    private readonly Mock<INovelRepository> _mockRepository;
    private readonly Mock<ILabelRepository> _mockLabelRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly NovelDtoMapper _mapper;
    private readonly PatchNovelHandler _handler;

    public PatchNovelHandlerTests()
    {
        _mockRepository = new Mock<INovelRepository>();
        _mockLabelRepo = new Mock<ILabelRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mapper = new NovelDtoMapper();
        _handler = new PatchNovelHandler(_mockRepository.Object, _mockLabelRepo.Object, _mockUnitOfWork.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesNovel()
    {
        // Arrange
        var novel = Novel.Create("Original Title");
        novel.InitializeStartLabel("start"); // Initialize StartLabel to avoid null reference
        var novelId = novel.Id; // Use the actual novel's ID
        var command = new PatchNovelCommand { NovelId = novelId, Title = "Updated Title" };

        _mockRepository
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

        _mockRepository
            .Setup(r => r.AddOrUpdateAsync(novel, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.BeginTransaction());
        _mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
        _mockRepository.Verify(r => r.AddOrUpdateAsync(novel, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingNovel_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var command = new PatchNovelCommand { NovelId = novelId, Title = "Updated" };

        _mockRepository
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Novel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        _mockRepository.Verify(r => r.AddOrUpdateAsync(It.IsAny<Novel>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
