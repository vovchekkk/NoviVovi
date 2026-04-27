using Moq;
using NoviVovi.Application.Characters.Abstactions;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Features.Add;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Tests.Characters;

public class AddCharacterHandlerTests
{
    private readonly Mock<INovelRepository> _mockNovelRepo;
    private readonly Mock<ICharacterRepository> _mockCharacterRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<CharacterDtoMapper> _mockMapper;
    private readonly AddCharacterHandler _handler;

    public AddCharacterHandlerTests()
    {
        _mockNovelRepo = new Mock<INovelRepository>();
        _mockCharacterRepo = new Mock<ICharacterRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<CharacterDtoMapper>();
        
        _handler = new AddCharacterHandler(
            _mockNovelRepo.Object,
            _mockCharacterRepo.Object,
            _mockUnitOfWork.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsCharacter()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var novel = Novel.Create("Test Novel");
        var command = new AddCharacterCommand 
        { 
            NovelId = novelId, 
            Name = "Alice", 
            NameColor = "FF5733", 
            Description = "Main character" 
        };
        var expectedDto = new CharacterDto(Guid.NewGuid(), "Alice", "FF5733", "Main character", new List<CharacterStateDto>());

        _mockNovelRepo
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

        _mockCharacterRepo
            .Setup(r => r.AddOrUpdateAsync(It.IsAny<Character>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockNovelRepo
            .Setup(r => r.AddOrUpdateAsync(It.IsAny<Novel>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockMapper
            .Setup(m => m.ToDto(It.IsAny<Character>()))
            .Returns(expectedDto);

        _mockUnitOfWork.Setup(u => u.BeginTransaction());
        _mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alice", result.Name);
        Assert.Equal("FF5733", result.NameColor);
        
        _mockCharacterRepo.Verify(r => r.AddOrUpdateAsync(It.IsAny<Character>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingNovel_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var command = new AddCharacterCommand 
        { 
            NovelId = novelId, 
            Name = "Alice", 
            NameColor = "FF5733" 
        };

        _mockNovelRepo
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Novel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        _mockCharacterRepo.Verify(r => r.AddOrUpdateAsync(It.IsAny<Character>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
