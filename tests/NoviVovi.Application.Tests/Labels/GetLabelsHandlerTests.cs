using Moq;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Labels.Dtos;
using NoviVovi.Application.Labels.Features.Get;
using NoviVovi.Application.Labels.Mappers;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Tests.Labels;

public class GetLabelsHandlerTests
{
    private readonly Mock<INovelRepository> _mockNovelRepo;
    private readonly Mock<ILabelRepository> _mockLabelRepo;
    private readonly Mock<LabelDtoMapper> _mockMapper;
    private readonly GetLabelsHandler _handler;

    public GetLabelsHandlerTests()
    {
        _mockNovelRepo = new Mock<INovelRepository>();
        _mockLabelRepo = new Mock<ILabelRepository>();
        _mockMapper = new Mock<LabelDtoMapper>();
        _handler = new GetLabelsHandler(_mockNovelRepo.Object, _mockLabelRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ExistingNovel_ReturnsLabels()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var novel = Novel.Create("Test Novel");
        
        var expectedDtos = new List<LabelDto>
        {
            new LabelDto(Guid.NewGuid(), "chapter1", novelId, new List<StepDto>()),
            new LabelDto(Guid.NewGuid(), "chapter2", novelId, new List<StepDto>())
        };

        _mockNovelRepo
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

        _mockMapper
            .Setup(m => m.ToDtos(It.IsAny<IEnumerable<Label>>()))
            .Returns(expectedDtos);

        var query = new GetLabelsQuery(novelId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockNovelRepo.Verify(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingNovel_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();

        _mockNovelRepo
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Novel?)null);

        var query = new GetLabelsQuery(novelId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(query, CancellationToken.None));
    }
}
