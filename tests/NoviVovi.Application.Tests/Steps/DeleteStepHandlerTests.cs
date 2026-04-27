using Moq;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Steps.Features.Delete;
using NoviVovi.Domain.Labels;

namespace NoviVovi.Application.Tests.Steps;

public class DeleteStepHandlerTests
{
    private readonly Mock<ILabelRepository> _mockLabelRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly DeleteStepHandler _handler;

    public DeleteStepHandlerTests()
    {
        _mockLabelRepo = new Mock<ILabelRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteStepHandler(_mockLabelRepo.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ExistingStep_DeletesSuccessfully()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var label = Label.Create("chapter1", novelId);
        var labelId = label.Id;
        
        // Add a step to the label so it exists in the collection
        var step = Domain.Steps.JumpStep.Create(label); // Jump to itself for simplicity
        label.AddStep(step);
        var stepId = step.Id;

        _mockLabelRepo
            .Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(label);

        _mockLabelRepo
            .Setup(r => r.AddOrUpdateAsync(label, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var command = new DeleteStepCommand(novelId, labelId, stepId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockLabelRepo.Verify(r => r.AddOrUpdateAsync(label, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingLabel_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var labelId = Guid.NewGuid();
        var stepId = Guid.NewGuid();

        _mockLabelRepo
            .Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Label?)null);

        var command = new DeleteStepCommand(novelId, labelId, stepId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        _mockLabelRepo.Verify(r => r.AddOrUpdateAsync(It.IsAny<Label>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
