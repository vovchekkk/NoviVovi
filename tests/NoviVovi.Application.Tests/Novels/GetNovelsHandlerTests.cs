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
    private readonly NovelDtoMapper _mockMapper;
    private readonly GetNovelsHandler _handler;

    public GetNovelsHandlerTests()
    {
        _mockRepository = new Mock<INovelRepository>();
        _mockMapper = new NovelDtoMapper();
        _handler = new GetNovelsHandler(_mockRepository.Object, _mockMapper);
    }

    [Fact]
    public async Task Handle_NovelsExist_ReturnsDtos()
    {
        // Arrange
        var novel1 = Novel.Create("Novel 1");
        novel1.InitializeStartLabel("start");
        
        var novel2 = Novel.Create("Novel 2");
        novel2.InitializeStartLabel("start");
        
        var novel3 = Novel.Create("Novel 3");
        novel3.InitializeStartLabel("start");
        
        var novels = new List<Novel> { novel1, novel2, novel3 };

        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(novels);

        var query = new GetNovelsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
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


        var query = new GetNovelsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
