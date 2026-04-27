using Moq;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Labels.Dtos;
using NoviVovi.Application.Labels.Features.Add;
using NoviVovi.Application.Labels.Mappers;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Tests.Labels;

public class AddLabelHandlerTests
{
    private readonly Mock<INovelRepository> _mockNovelRepo;
    private readonly Mock<ILabelRepository> _mockLabelRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IStorageService> _mockStorageService;
    private readonly LabelDtoMapper _mockMapper;
    private readonly AddLabelHandler _handler;

    public AddLabelHandlerTests()
    {
        _mockNovelRepo = new Mock<INovelRepository>();
        _mockLabelRepo = new Mock<ILabelRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
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
        
        _handler = new AddLabelHandler(
            _mockNovelRepo.Object,
            _mockLabelRepo.Object,
            _mockUnitOfWork.Object,
            _mockMapper
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsLabel()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var novel = Novel.Create("Test Novel");
        var command = new AddLabelCommand { NovelId = novelId, Name = "chapter1" };
        var expectedDto = new LabelDto(Guid.NewGuid(), "chapter1", novelId, new List<StepDto>());

        _mockNovelRepo
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

        _mockLabelRepo
            .Setup(r => r.AddOrUpdateAsync(It.IsAny<Label>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockNovelRepo
            .Setup(r => r.AddOrUpdateAsync(novel, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);


        _mockUnitOfWork.Setup(u => u.BeginTransaction());
        _mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("chapter1", result.Name);
        
        _mockNovelRepo.Verify(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()), Times.Once);
        _mockLabelRepo.Verify(r => r.AddOrUpdateAsync(It.IsAny<Label>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingNovel_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var command = new AddLabelCommand { NovelId = novelId, Name = "chapter1" };

        _mockNovelRepo
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Novel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        _mockLabelRepo.Verify(r => r.AddOrUpdateAsync(It.IsAny<Label>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
