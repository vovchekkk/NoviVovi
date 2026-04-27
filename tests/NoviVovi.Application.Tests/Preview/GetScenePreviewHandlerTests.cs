using Moq;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Preview.Dtos;
using NoviVovi.Application.Preview.Features.Get;
using NoviVovi.Application.Preview.Mappers;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Domain.Labels;

namespace NoviVovi.Application.Tests.Preview;

public class GetScenePreviewHandlerTests
{
    private readonly Mock<ILabelRepository> _mockLabelRepo;
    private readonly Mock<SceneStateDtoMapper> _mockMapper;
    private readonly GetScenePreviewHandler _handler;

    public GetScenePreviewHandlerTests()
    {
        _mockLabelRepo = new Mock<ILabelRepository>();
        _mockMapper = new Mock<SceneStateDtoMapper>();
        _handler = new GetScenePreviewHandler(_mockLabelRepo.Object, _mockMapper.Object);
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

        _mockMapper
            .Setup(m => m.ToDto(It.IsAny<Domain.Preview.VisualSnapshot>()))
            .Returns(expectedDto);

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
