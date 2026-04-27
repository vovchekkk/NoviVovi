using Moq;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Novels.Features.Get;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Mappers;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Tests.Novels;

public class GetNovelHandlerTests
{
    private readonly Mock<INovelRepository> _mockRepository;
    private readonly Mock<NovelDtoMapper> _mockMapper;
    private readonly GetNovelHandler _handler;

    public GetNovelHandlerTests()
    {
        _mockRepository = new Mock<INovelRepository>();
        _mockMapper = new Mock<NovelDtoMapper>();
        _handler = new GetNovelHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ExistingNovel_ReturnsDto()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var novel = Novel.Create("Test Novel");
        var expectedDto = new NovelDto(novelId, "Test Novel", Guid.NewGuid(), new List<Guid>(), new List<Guid>());

        _mockRepository
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

        _mockMapper
            .Setup(m => m.ToDto(novel))
            .Returns(expectedDto);

        var query = new GetNovelQuery(novelId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Novel", result.Title);
        _mockRepository.Verify(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingNovel_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Novel?)null);

        var query = new GetNovelQuery(novelId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(query, CancellationToken.None));
    }
}
