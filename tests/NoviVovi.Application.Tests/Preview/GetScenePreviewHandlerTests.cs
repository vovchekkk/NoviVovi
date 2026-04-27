using Moq;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Dialogue.Mappers;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Preview.Dtos;
using NoviVovi.Application.Preview.Features.Get;
using NoviVovi.Application.Preview.Mappers;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Domain.Labels;

namespace NoviVovi.Application.Tests.Preview;

public class GetScenePreviewHandlerTests
{
    private readonly Mock<ILabelRepository> _mockLabelRepo;
    private readonly Mock<IStorageService> _mockStorageService;
    private readonly SceneStateDtoMapper _mockMapper;
    private readonly GetScenePreviewHandler _handler;

    public GetScenePreviewHandlerTests()
    {
        _mockLabelRepo = new Mock<ILabelRepository>();
        _mockStorageService = new Mock<IStorageService>();
        
        // Setup storage service mock
        _mockStorageService.Setup(s => s.GetViewUrl(It.IsAny<string>())).Returns("https://test.com/view");
        
        // SceneStateDtoMapper requires complex dependency chain
        var sizeMapper = new SizeDtoMapper();
        var imageMapper = new ImageDtoMapper(_mockStorageService.Object, sizeMapper);
        var transformMapper = new TransformDtoMapper();
        var characterStateMapper = new CharacterStateDtoMapper(imageMapper, transformMapper);
        var characterMapper = new CharacterDtoMapper(characterStateMapper);
        var replicaMapper = new ReplicaDtoMapper(characterMapper);
        _mockMapper = new SceneStateDtoMapper(replicaMapper, characterMapper, imageMapper, transformMapper);
        
        _handler = new GetScenePreviewHandler(_mockLabelRepo.Object, _mockMapper);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsSceneState()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var labelId = Guid.NewGuid();
        var stepId = Guid.NewGuid();
        var label = Label.Create("chapter1", novelId);
        
        var query = new GetScenePreviewQuery(novelId, labelId, stepId);
        var expectedDto = new SceneStateDto(null, null, null, Array.Empty<CharacterObjectDto>());

        _mockLabelRepo
            .Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(label);


        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _mockLabelRepo.Verify(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingLabel_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var labelId = Guid.NewGuid();
        var stepId = Guid.NewGuid();
        var query = new GetScenePreviewQuery(novelId, labelId, stepId);

        _mockLabelRepo
            .Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Label?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(query, CancellationToken.None));
    }
}
