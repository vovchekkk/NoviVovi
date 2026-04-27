using Moq;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Features.Create;
using NoviVovi.Application.Novels.Mappers;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Tests.Novels;

public class CreateNovelHandlerTests
{
    private readonly Mock<INovelRepository> _mockNovelRepo;
    private readonly Mock<ILabelRepository> _mockLabelRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<NovelDtoMapper> _mockMapper;
    private readonly CreateNovelHandler _handler;

    public CreateNovelHandlerTests()
    {
        _mockNovelRepo = new Mock<INovelRepository>();
        _mockLabelRepo = new Mock<ILabelRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<NovelDtoMapper>();
        _handler = new CreateNovelHandler(_mockNovelRepo.Object, _mockLabelRepo.Object, _mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesNovel()
    {
        // Arrange
        var command = new CreateNovelCommand { Title = "New Novel" };
        var expectedDto = new NovelDto(Guid.NewGuid(), "New Novel", Guid.NewGuid(), new List<Guid>(), new List<Guid>());

        _mockNovelRepo
            .Setup(r => r.AddOrUpdateAsync(It.IsAny<Novel>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockLabelRepo
            .Setup(r => r.AddOrUpdateAsync(It.IsAny<Domain.Labels.Label>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockMapper
            .Setup(m => m.ToDto(It.IsAny<Novel>()))
            .Returns(expectedDto);

        _mockUnitOfWork.Setup(u => u.BeginTransaction());
        _mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Novel", result.Title);
        _mockNovelRepo.Verify(r => r.AddOrUpdateAsync(It.IsAny<Novel>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_RollsBack()
    {
        // Arrange
        var command = new CreateNovelCommand { Title = "New Novel" };

        _mockNovelRepo
            .Setup(r => r.AddOrUpdateAsync(It.IsAny<Novel>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        _mockUnitOfWork.Setup(u => u.BeginTransaction());
        _mockUnitOfWork.Setup(u => u.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        _mockUnitOfWork.Verify(u => u.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
