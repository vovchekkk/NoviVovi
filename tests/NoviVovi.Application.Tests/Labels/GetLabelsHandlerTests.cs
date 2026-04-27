using Moq;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Labels.Dtos;
using NoviVovi.Application.Labels.Features.Get;
using NoviVovi.Application.Labels.Mappers;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Tests.Labels;

public class GetLabelsHandlerTests
{
    private readonly Mock<INovelRepository> _mockNovelRepo;
    private readonly Mock<ILabelRepository> _mockLabelRepo;
    private readonly Mock<IStorageService> _mockStorageService;
    private readonly LabelDtoMapper _mockMapper;
    private readonly GetLabelsHandler _handler;

    public GetLabelsHandlerTests()
    {
        _mockNovelRepo = new Mock<INovelRepository>();
        _mockLabelRepo = new Mock<ILabelRepository>();
        _mockStorageService = new Mock<IStorageService>();
        _mockStorageService.Setup(s => s.GetViewUrl(It.IsAny<string>())).Returns("https://test.com/view");
                
        // LabelDtoMapper requires StepDtoMapper, which requires other mappers
        var sizeMapper = new SizeDtoMapper();
        var imageMapper = new ImageDtoMapper(_mockStorageService.Object, sizeMapper);
        var transformMapper = new TransformDtoMapper();
        var characterStateMapper = new CharacterStateDtoMapper(imageMapper, transformMapper);
        var characterMapper = new CharacterDtoMapper(characterStateMapper);
        var stepMapper = new StepDtoMapper(characterMapper, imageMapper, transformMapper);
        _mockMapper = new LabelDtoMapper(stepMapper);
        _handler = new GetLabelsHandler(_mockNovelRepo.Object, _mockLabelRepo.Object, _mockMapper);
    }

    [Fact]
    public async Task Handle_ExistingNovel_ReturnsLabels()
    {
        // Arrange
        var novel = Novel.Create("Test Novel");
        var novelId = novel.Id;
        
        // Add labels to novel
        var label1 = Domain.Labels.Label.Create("chapter1", novelId);
        var label2 = Domain.Labels.Label.Create("chapter2", novelId);
        novel.AddLabel(label1);
        novel.AddLabel(label2);

        _mockNovelRepo
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

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
