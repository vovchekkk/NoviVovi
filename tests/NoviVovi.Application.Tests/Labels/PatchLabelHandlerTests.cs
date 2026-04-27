using Moq;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Labels.Dtos;
using NoviVovi.Application.Labels.Features.Patch;
using NoviVovi.Application.Labels.Mappers;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Tests.Labels;

public class PatchLabelHandlerTests
{
    private readonly Mock<ILabelRepository> _mockLabelRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<LabelDtoMapper> _mockMapper;
    private readonly PatchLabelHandler _handler;

    public PatchLabelHandlerTests()
    {
        _mockLabelRepo = new Mock<ILabelRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<LabelDtoMapper>();
        
        _handler = new PatchLabelHandler(
            _mockLabelRepo.Object,
            _mockUnitOfWork.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesLabel()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var labelId = Guid.NewGuid();
        var label = Label.Create("original_name", novelId);
        var command = new PatchLabelCommand 
        { 
            NovelId = novelId, 
            LabelId = labelId,
            Name = "updated_name"
        };
        var expectedDto = new LabelDto(labelId, "updated_name", novelId, new List<StepDto>());

        _mockLabelRepo
            .Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(label);

        _mockLabelRepo
            .Setup(r => r.AddOrUpdateAsync(label, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockMapper
            .Setup(m => m.ToDto(label))
            .Returns(expectedDto);

        _mockUnitOfWork.Setup(u => u.BeginTransaction());
        _mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("updated_name", result.Name);
        
        _mockLabelRepo.Verify(r => r.AddOrUpdateAsync(label, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingLabel_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var labelId = Guid.NewGuid();
        var command = new PatchLabelCommand 
        { 
            NovelId = novelId, 
            LabelId = labelId,
            Name = "updated"
        };

        _mockLabelRepo
            .Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Label?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        _mockLabelRepo.Verify(r => r.AddOrUpdateAsync(It.IsAny<Label>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
