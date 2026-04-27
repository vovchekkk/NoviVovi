using Moq;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Features.Get;
using NoviVovi.Application.Novels.Mappers;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Tests.Novels;

public class GetNovelsHandlerTests
{
    private readonly Mock<INovelRepository> _mockRepository;
    private readonly Mock<NovelDtoMapper> _mockMapper;
    private readonly GetNovelsHandler _handler;

    public GetNovelsHandlerTests()
    {
        _mockRepository = new Mock<INovelRepository>();
        _mockMapper = new Mock<NovelDtoMapper>();
        _handler = new GetNovelsHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_NovelsExist_ReturnsDtos()
    {
        // Arrange
        var novels = new List<Novel>
        {
            Novel.Create("Novel 1"),
            Novel.Create("Novel 2"),
            Novel.Create("Novel 3")
        };

        var expectedDtos = new List<NovelDto>
        {
            new NovelDto(Guid.NewGuid(), "Novel 1", Guid.NewGuid(), new List<Guid>(), new List<Guid>()),
            new NovelDto(Guid.NewGuid(), "Novel 2", Guid.NewGuid(), new List<Guid>(), new List<Guid>()),
            new NovelDto(Guid.NewGuid(), "Novel 3", Guid.NewGuid(), new List<Guid>(), new List<Guid>())
        };

        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(novels);

        _mockMapper
            .Setup(m => m.ToDtos(novels))
            .Returns(expectedDtos);

        var query = new GetNovelsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(m => m.ToDtos(novels), Times.Once);
    }

    [Fact]
    public async Task Handle_NoNovels_ReturnsEmptyList()
    {
        // Arrange
        var novels = new List<Novel>();
        var expectedDtos = new List<NovelDto>();

        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(novels);

        _mockMapper
            .Setup(m => m.ToDtos(novels))
            .Returns(expectedDtos);

        var query = new GetNovelsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
