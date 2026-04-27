using Moq;
using NoviVovi.Application.Characters.Abstactions;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Features.Get;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Domain.Characters;

namespace NoviVovi.Application.Tests.Characters;

public class GetCharacterStatesHandlerTests
{
    private readonly Mock<INovelRepository> _mockNovelRepo;
    private readonly Mock<ICharacterRepository> _mockCharacterRepo;
    private readonly Mock<CharacterStateDtoMapper> _mockMapper;
    private readonly GetCharacterStatesHandler _handler;

    public GetCharacterStatesHandlerTests()
    {
        _mockNovelRepo = new Mock<INovelRepository>();
        _mockCharacterRepo = new Mock<ICharacterRepository>();
        _mockMapper = new Mock<CharacterStateDtoMapper>();
        _handler = new GetCharacterStatesHandler(_mockNovelRepo.Object, _mockCharacterRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ExistingCharacter_ReturnsStates()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var characterId = Guid.NewGuid();
        var character = Character.Create("Alice", novelId, Domain.Common.Color.FromHex("FF5733"), null);
        
        var expectedDtos = new List<CharacterStateDto>();

        _mockCharacterRepo
            .Setup(r => r.GetByIdAsync(characterId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        _mockMapper
            .Setup(m => m.ToDtos(It.IsAny<IEnumerable<CharacterState>>()))
            .Returns(expectedDtos);

        var query = new GetCharacterStatesQuery(novelId, characterId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _mockCharacterRepo.Verify(r => r.GetByIdAsync(characterId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingCharacter_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var characterId = Guid.NewGuid();

        _mockCharacterRepo
            .Setup(r => r.GetByIdAsync(characterId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Character?)null);

        var query = new GetCharacterStatesQuery(novelId, characterId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(query, CancellationToken.None));
    }
}
