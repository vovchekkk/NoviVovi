using Moq;
using NoviVovi.Application.Characters.Abstactions;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Features.Get;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Domain.Characters;

namespace NoviVovi.Application.Tests.Characters;

public class GetCharactersHandlerTests
{
    private readonly Mock<INovelRepository> _mockNovelRepo;
    private readonly Mock<ICharacterRepository> _mockCharacterRepo;
    private readonly Mock<CharacterDtoMapper> _mockMapper;
    private readonly GetCharactersHandler _handler;

    public GetCharactersHandlerTests()
    {
        _mockNovelRepo = new Mock<INovelRepository>();
        _mockCharacterRepo = new Mock<ICharacterRepository>();
        _mockMapper = new Mock<CharacterDtoMapper>();
        _handler = new GetCharactersHandler(_mockNovelRepo.Object, _mockCharacterRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ExistingCharacters_ReturnsDtos()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var characters = new List<Character>
        {
            Character.Create("Alice", novelId, Domain.Common.Color.FromHex("FF5733"), null),
            Character.Create("Bob", novelId, Domain.Common.Color.FromHex("00FF00"), null)
        };

        var expectedDtos = new List<CharacterDto>
        {
            new CharacterDto(Guid.NewGuid(), "Alice", "FF5733", null, new List<CharacterStateDto>()),
            new CharacterDto(Guid.NewGuid(), "Bob", "00FF00", null, new List<CharacterStateDto>())
        };

        _mockCharacterRepo
            .Setup(r => r.GetAllByNovelIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(characters);

        _mockMapper
            .Setup(m => m.ToDtos(characters))
            .Returns(expectedDtos);

        var query = new GetCharactersQuery(novelId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockCharacterRepo.Verify(r => r.GetAllByNovelIdAsync(novelId, It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(m => m.ToDtos(characters), Times.Once);
    }

    [Fact]
    public async Task Handle_NoCharacters_ReturnsEmptyList()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var characters = new List<Character>();
        var expectedDtos = new List<CharacterDto>();

        _mockCharacterRepo
            .Setup(r => r.GetAllByNovelIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(characters);

        _mockMapper
            .Setup(m => m.ToDtos(characters))
            .Returns(expectedDtos);

        var query = new GetCharactersQuery(novelId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockCharacterRepo.Verify(r => r.GetAllByNovelIdAsync(novelId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
