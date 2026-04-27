using Moq;
using NoviVovi.Application.Characters.Abstactions;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Features.Patch;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Images.Abstractions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Scene;

namespace NoviVovi.Application.Tests.Characters;

public class PatchCharacterStateHandlerTests
{
    private readonly Mock<INovelRepository> _mockNovelRepo;
    private readonly Mock<ICharacterRepository> _mockCharacterRepo;
    private readonly Mock<IImageRepository> _mockImageRepo;
    private readonly Mock<TransformDtoMapper> _mockTransformMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<CharacterStateDtoMapper> _mockMapper;
    private readonly PatchCharacterStateHandler _handler;

    public PatchCharacterStateHandlerTests()
    {
        _mockNovelRepo = new Mock<INovelRepository>();
        _mockCharacterRepo = new Mock<ICharacterRepository>();
        _mockImageRepo = new Mock<IImageRepository>();
        _mockTransformMapper = new Mock<TransformDtoMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<CharacterStateDtoMapper>();
        
        _handler = new PatchCharacterStateHandler(
            _mockNovelRepo.Object,
            _mockCharacterRepo.Object,
            _mockImageRepo.Object,
            _mockTransformMapper.Object,
            _mockUnitOfWork.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesCharacterState()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var characterId = Guid.NewGuid();
        var stateId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        
        var novel = Novel.Create("Test Novel");
        var character = Character.Create("Alice", novelId, Domain.Common.Color.FromHex("FF5733"), null);
        var image = Image.CreatePending("test.png", novelId, "path/test.png", "png", ImageType.Character, new Size(512, 512));
        
        var command = new PatchCharacterStateCommand 
        { 
            NovelId = novelId,
            CharacterId = characterId,
            StateId = stateId,
            Name = "updated_happy",
            ImageId = imageId
        };
        
        var expectedDto = new CharacterStateDto(stateId, "updated_happy", null, null, null);

        _mockNovelRepo
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

        _mockCharacterRepo
            .Setup(r => r.GetByIdAsync(characterId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        _mockImageRepo
            .Setup(r => r.GetByIdAsync(imageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(image);

        _mockCharacterRepo
            .Setup(r => r.AddOrUpdateAsync(character, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockMapper
            .Setup(m => m.ToDto(It.IsAny<CharacterState>()))
            .Returns(expectedDto);

        _mockUnitOfWork.Setup(u => u.BeginTransaction());
        _mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
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
        
        var command = new PatchCharacterStateCommand 
        { 
            NovelId = novelId,
            CharacterId = characterId,
            StateId = stateId,
            Name = "updated"
        };

        _mockNovelRepo
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

        _mockCharacterRepo
            .Setup(r => r.GetByIdAsync(characterId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Character?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        _mockCharacterRepo.Verify(r => r.AddOrUpdateAsync(It.IsAny<Character>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
