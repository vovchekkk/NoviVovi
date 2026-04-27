using Moq;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Features.Get;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Labels;

namespace NoviVovi.Application.Tests.Steps;

public class GetStepHandlerTests
{
    private readonly Mock<ILabelRepository> _mockLabelRepo;
    private readonly Mock<IStorageService> _mockStorageService;
    private readonly StepDtoMapper _mockMapper;
    private readonly GetStepHandler _handler;

    public GetStepHandlerTests()
    {
        _mockLabelRepo = new Mock<ILabelRepository>();
        _mockStorageService = new Mock<IStorageService>();
        _mockStorageService.Setup(s => s.GetViewUrl(It.IsAny<string>())).Returns("https://test.com/view");
        
        // StepDtoMapper requires dependencies with full chain
        var sizeMapper = new SizeDtoMapper();
        var imageMapper = new ImageDtoMapper(_mockStorageService.Object, sizeMapper);
        var transformMapper = new TransformDtoMapper();
        var characterStateMapper = new CharacterStateDtoMapper(imageMapper, transformMapper);
        var characterMapper = new CharacterDtoMapper(characterStateMapper);
        _mockMapper = new StepDtoMapper(characterMapper, imageMapper, transformMapper);
        
        _handler = new GetStepHandler(_mockLabelRepo.Object, _mockMapper);
    }

    [Fact]
    public async Task Handle_ExistingStep_ReturnsDto()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var label = Label.Create("chapter1", novelId);
        var labelId = label.Id;
        
        // Create and add step to label
        var step = Domain.Steps.JumpStep.Create(label);
        label.AddStep(step);
        var stepId = step.Id;
        
        var query = new GetStepQuery(novelId, labelId, stepId);

        _mockLabelRepo
            .Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(label);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(stepId, result.Id);
        _mockLabelRepo.Verify(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingLabel_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var labelId = Guid.NewGuid();
        var stepId = Guid.NewGuid();
        var query = new GetStepQuery(novelId, labelId, stepId);

        _mockLabelRepo
            .Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Label?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(query, CancellationToken.None));
    }
}
