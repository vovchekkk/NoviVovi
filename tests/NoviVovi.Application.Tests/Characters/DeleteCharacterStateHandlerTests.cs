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

public class DeleteCharacterStateHandlerTests
{
    private readonly Mock<INovelRepository> _mockNovelRepo;
    private readonly Mock<ICharacterRepository> _mockCharacterRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly DeleteCharacterStateHandler _handler;

    public DeleteCharacterStateHandlerTests()
    {
        _mockNovelRepo = new Mock<INovelRepository>();
        _mockCharacterRepo = new Mock<ICharacterRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        
        _handler = new DeleteCharacterStateHandler(
            _mockNovelRepo.Object,
            _mockCharacterRepo.Object,
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_ExistingCharacterState_DeletesSuccessfully()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var novel = Novel.Create("Test Novel");
        
        // Create character and add to novel
        var character = Character.Create("Alice", novelId, Domain.Common.Color.FromHex("FF5733"), null);
        novel.AddCharacter(character);
        var characterId = character.Id;
        
        // Create required dependencies for CharacterState
        var image = Domain.Images.Image.CreatePending("test.png", novelId, "path/test.png", "png", Domain.Images.ImageType.Character, new Domain.Scene.Size(100, 100));
        var transform = Domain.Scene.Transform.Create(new Domain.Scene.Position(0, 0), new Domain.Scene.Size(100, 100));
        
        // Add state to character
        var state = CharacterState.Create("happy", image, transform);
        character.AddCharacterState(state);
        var stateId = state.Id;

        _mockNovelRepo
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

        _mockCharacterRepo
            .Setup(r => r.GetByIdAsync(characterId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        _mockCharacterRepo
            .Setup(r => r.AddOrUpdateAsync(character, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.BeginTransaction());
        _mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var command = new DeleteCharacterStateCommand(novelId, characterId, stateId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockCharacterRepo.Verify(r => r.AddOrUpdateAsync(character, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingCharacter_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var characterId = Guid.NewGuid();
        var stateId = Guid.NewGuid();
        
        var novel = Novel.Create("Test Novel");

        _mockNovelRepo
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

        _mockCharacterRepo
            .Setup(r => r.GetByIdAsync(characterId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Character?)null);

        var command = new DeleteCharacterStateCommand(novelId, characterId, stateId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        _mockCharacterRepo.Verify(r => r.AddOrUpdateAsync(It.IsAny<Character>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
