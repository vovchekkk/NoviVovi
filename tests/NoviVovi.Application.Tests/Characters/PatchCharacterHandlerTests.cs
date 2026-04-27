using Moq;
using NoviVovi.Application.Characters.Abstactions;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Features.Patch;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Tests.Characters;

public class PatchCharacterHandlerTests
{
    private readonly Mock<INovelRepository> _mockNovelRepo;
    private readonly Mock<ICharacterRepository> _mockCharacterRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IStorageService> _mockStorageService;
    private readonly CharacterDtoMapper _mockMapper;
    private readonly PatchCharacterHandler _handler;

    public PatchCharacterHandlerTests()
    {
        _mockNovelRepo = new Mock<INovelRepository>();
        _mockCharacterRepo = new Mock<ICharacterRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockStorageService = new Mock<IStorageService>();
        _mockStorageService.Setup(s => s.GetViewUrl(It.IsAny<string>())).Returns("https://test.com/view");
                
        // CharacterDtoMapper requires CharacterStateDtoMapper
        var sizeMapper = new SizeDtoMapper();
        var imageMapper = new ImageDtoMapper(_mockStorageService.Object, sizeMapper);
        var transformMapper = new TransformDtoMapper();
        var characterStateMapper = new CharacterStateDtoMapper(imageMapper, transformMapper);
        _mockMapper = new CharacterDtoMapper(characterStateMapper);
        
        _handler = new PatchCharacterHandler(
            _mockNovelRepo.Object,
            _mockCharacterRepo.Object,
            _mockUnitOfWork.Object,
            _mockMapper
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesCharacter()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var characterId = Guid.NewGuid();
        var novel = Novel.Create("Test Novel");
        var character = Character.Create("Alice", novelId, Domain.Common.Color.FromHex("FF5733"), "Original");
        var command = new PatchCharacterCommand 
        { 
            NovelId = novelId, 
            CharacterId = characterId,
            Name = "Alice Updated",
            NameColor = "00FF00",
            Description = "Updated description"
        };
        var expectedDto = new CharacterDto(characterId, "Alice Updated", "00FF00", "Updated description", new List<CharacterStateDto>());

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

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alice Updated", result.Name);
        
        _mockCharacterRepo.Verify(r => r.AddOrUpdateAsync(character, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingCharacter_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var characterId = Guid.NewGuid();
        var novel = Novel.Create("Test Novel");
        var command = new PatchCharacterCommand 
        { 
            NovelId = novelId, 
            CharacterId = characterId,
            Name = "Updated"
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
