using Moq;
using NoviVovi.Application.Characters.Abstactions;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Features.Get;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Domain.Characters;

namespace NoviVovi.Application.Tests.Characters;

public class GetCharacterHandlerTests
{
    private readonly Mock<INovelRepository> _mockNovelRepo;
    private readonly Mock<ICharacterRepository> _mockCharacterRepo;
    private readonly Mock<CharacterDtoMapper> _mockMapper;
    private readonly GetCharacterHandler _handler;

    public GetCharacterHandlerTests()
    {
        _mockNovelRepo = new Mock<INovelRepository>();
        _mockCharacterRepo = new Mock<ICharacterRepository>();
        _mockMapper = new Mock<CharacterDtoMapper>();
        _handler = new GetCharacterHandler(_mockNovelRepo.Object, _mockCharacterRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ExistingCharacter_ReturnsDto()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var characterId = Guid.NewGuid();
        var character = Character.Create("Alice", novelId, Domain.Common.Color.FromHex("FF5733"), null);
        var expectedDto = new CharacterDto(characterId, "Alice", "FF5733", null, new List<CharacterStateDto>());

        _mockCharacterRepo
            .Setup(r => r.GetByIdAsync(characterId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        _mockMapper
            .Setup(m => m.ToDto(character))
            .Returns(expectedDto);

        var query = new GetCharacterQuery(novelId, characterId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alice", result.Name);
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

        var query = new GetCharacterQuery(novelId, characterId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(query, CancellationToken.None));
    }
}
