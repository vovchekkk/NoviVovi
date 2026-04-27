using Moq;
using NoviVovi.Application.Characters.Abstactions;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Features.Get;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Domain.Characters;

namespace NoviVovi.Application.Tests.Characters;

public class GetCharacterStateHandlerTests
{
    private readonly Mock<INovelRepository> _mockNovelRepo;
    private readonly Mock<ICharacterRepository> _mockCharacterRepo;
    private readonly Mock<IStorageService> _mockStorageService;
    private readonly CharacterStateDtoMapper _mockMapper;
    private readonly GetCharacterStateHandler _handler;

    public GetCharacterStateHandlerTests()
    {
        _mockNovelRepo = new Mock<INovelRepository>();
        _mockCharacterRepo = new Mock<ICharacterRepository>();
        _mockStorageService = new Mock<IStorageService>();
        _mockStorageService.Setup(s => s.GetViewUrl(It.IsAny<string>())).Returns("https://test.com/view");
                
        // CharacterStateDtoMapper requires dependencies
        var sizeMapper = new SizeDtoMapper();
        var imageMapper = new ImageDtoMapper(_mockStorageService.Object, sizeMapper);
        var transformMapper = new TransformDtoMapper();
        _mockMapper = new CharacterStateDtoMapper(imageMapper, transformMapper);
        _handler = new GetCharacterStateHandler(_mockNovelRepo.Object, _mockCharacterRepo.Object, _mockMapper);
    }

    [Fact]
    public async Task Handle_ExistingCharacterState_ReturnsDto()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var character = Character.Create("Alice", novelId, Domain.Common.Color.FromHex("FF5733"), null);
        var characterId = character.Id;
        
        // Create and add state to character
        var image = Domain.Images.Image.CreatePending("test.png", novelId, "path/test.png", "png", Domain.Images.ImageType.Character, new Domain.Scene.Size(100, 100));
        var transform = Domain.Scene.Transform.Create(new Domain.Scene.Position(0, 0), new Domain.Scene.Size(100, 100));
        var state = CharacterState.Create("happy", image, transform);
        character.AddCharacterState(state);
        var stateId = state.Id;

        _mockCharacterRepo
            .Setup(r => r.GetByIdAsync(characterId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        var query = new GetCharacterStateQuery(novelId, characterId, stateId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("happy", result.Name);
        _mockCharacterRepo.Verify(r => r.GetByIdAsync(characterId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingCharacter_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var characterId = Guid.NewGuid();
        var stateId = Guid.NewGuid();

        _mockCharacterRepo
            .Setup(r => r.GetByIdAsync(characterId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Character?)null);

        var query = new GetCharacterStateQuery(novelId, characterId, stateId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(query, CancellationToken.None));
    }
}
