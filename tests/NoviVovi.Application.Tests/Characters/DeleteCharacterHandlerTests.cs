using Moq;
using NoviVovi.Application.Characters.Abstactions;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Features.Delete;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Tests.Characters;

public class DeleteCharacterHandlerTests
{
    private readonly Mock<INovelRepository> _mockNovelRepo;
    private readonly Mock<ICharacterRepository> _mockCharacterRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly DeleteCharacterHandler _handler;

    public DeleteCharacterHandlerTests()
    {
        _mockNovelRepo = new Mock<INovelRepository>();
        _mockCharacterRepo = new Mock<ICharacterRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        
        _handler = new DeleteCharacterHandler(
            _mockNovelRepo.Object,
            _mockCharacterRepo.Object,
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_ExistingCharacter_DeletesSuccessfully()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var characterId = Guid.NewGuid();
        var novel = Novel.Create("Test Novel");
        var character = Character.Create("Alice", novelId, Domain.Common.Color.FromHex("FF5733"), null);

        _mockNovelRepo
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

        _mockCharacterRepo
            .Setup(r => r.GetByIdAsync(characterId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        _mockCharacterRepo
            .Setup(r => r.DeleteAsync(character, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockNovelRepo
            .Setup(r => r.AddOrUpdateAsync(novel, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.BeginTransaction());
        _mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var command = new DeleteCharacterCommand(novelId, characterId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockCharacterRepo.Verify(r => r.GetByIdAsync(characterId, It.IsAny<CancellationToken>()), Times.Once);
        _mockCharacterRepo.Verify(r => r.DeleteAsync(character, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingCharacter_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var characterId = Guid.NewGuid();
        var novel = Novel.Create("Test Novel");

        _mockNovelRepo
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

        _mockCharacterRepo
            .Setup(r => r.GetByIdAsync(characterId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Character?)null);

        var command = new DeleteCharacterCommand(novelId, characterId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        _mockCharacterRepo.Verify(r => r.DeleteAsync(It.IsAny<Character>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
