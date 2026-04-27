using Moq;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Labels.Dtos;
using NoviVovi.Application.Labels.Features.Get;
using NoviVovi.Application.Labels.Mappers;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Domain.Labels;

namespace NoviVovi.Application.Tests.Labels;

public class GetLabelHandlerTests
{
    private readonly Mock<INovelRepository> _mockNovelRepo;
    private readonly Mock<ILabelRepository> _mockLabelRepo;
    private readonly Mock<LabelDtoMapper> _mockMapper;
    private readonly GetLabelHandler _handler;

    public GetLabelHandlerTests()
    {
        _mockNovelRepo = new Mock<INovelRepository>();
        _mockLabelRepo = new Mock<ILabelRepository>();
        _mockMapper = new Mock<LabelDtoMapper>();
        _handler = new GetLabelHandler(_mockNovelRepo.Object, _mockLabelRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ExistingLabel_ReturnsDto()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var labelId = Guid.NewGuid();
        var label = Label.Create("chapter1", novelId);
        var expectedDto = new LabelDto(labelId, "chapter1", novelId, new List<StepDto>());

        _mockLabelRepo
            .Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(label);

        _mockMapper
            .Setup(m => m.ToDto(label))
            .Returns(expectedDto);

        var query = new GetLabelQuery(novelId, labelId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("chapter1", result.Name);
        _mockLabelRepo.Verify(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingLabel_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var labelId = Guid.NewGuid();

        _mockLabelRepo
            .Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Label?)null);

        var query = new GetLabelQuery(novelId, labelId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(query, CancellationToken.None));
    }
}
