using Moq;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Features.Get;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Labels;

namespace NoviVovi.Application.Tests.Steps;

public class GetStepsHandlerTests
{
    private readonly Mock<ILabelRepository> _mockLabelRepo;
    private readonly Mock<StepDtoMapper> _mockMapper;
    private readonly GetStepsHandler _handler;

    public GetStepsHandlerTests()
    {
        _mockLabelRepo = new Mock<ILabelRepository>();
        _mockMapper = new Mock<StepDtoMapper>();
        _handler = new GetStepsHandler(_mockLabelRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ExistingLabel_ReturnsSteps()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var labelId = Guid.NewGuid();
        var label = Label.Create("chapter1", novelId);
        
        var query = new GetStepsQuery(novelId, labelId);
        var expectedDtos = new List<StepDto>();

        _mockLabelRepo
            .Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(label);

        _mockMapper
            .Setup(m => m.ToDtos(It.IsAny<IEnumerable<Domain.Steps.Step>>()))
            .Returns(expectedDtos);

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
        var query = new GetStepsQuery(novelId, labelId);

        _mockLabelRepo
            .Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Label?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(query, CancellationToken.None));
    }
}
