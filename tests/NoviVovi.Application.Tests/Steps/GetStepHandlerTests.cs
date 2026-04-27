using Moq;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Features.Get;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Labels;

namespace NoviVovi.Application.Tests.Steps;

public class GetStepHandlerTests
{
    private readonly Mock<ILabelRepository> _mockLabelRepo;
    private readonly Mock<StepDtoMapper> _mockMapper;
    private readonly GetStepHandler _handler;

    public GetStepHandlerTests()
    {
        _mockLabelRepo = new Mock<ILabelRepository>();
        _mockMapper = new Mock<StepDtoMapper>();
        _handler = new GetStepHandler(_mockLabelRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ExistingStep_ReturnsDto()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var labelId = Guid.NewGuid();
        var stepId = Guid.NewGuid();
        var label = Label.Create("chapter1", novelId);
        
        var query = new GetStepQuery(novelId, labelId, stepId);
        
        // Create a mock StepDto (using Moq to create abstract class instance)
        var expectedDto = Mock.Of<StepDto>(d => d.Id == stepId);

        _mockLabelRepo
            .Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(label);

        _mockMapper
            .Setup(m => m.ToDto(It.IsAny<Domain.Steps.Step>()))
            .Returns(expectedDto);

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
