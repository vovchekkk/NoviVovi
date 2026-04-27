using Moq;
using NoviVovi.Application.Characters.Abstactions;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Features.Add;
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

public class AddCharacterStateHandlerTests
{
    private readonly Mock<INovelRepository> _mockNovelRepo;
    private readonly Mock<ICharacterRepository> _mockCharacterRepo;
    private readonly Mock<IImageRepository> _mockImageRepo;
    private readonly Mock<TransformDtoMapper> _mockTransformMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<CharacterStateDtoMapper> _mockMapper;
    private readonly AddCharacterStateHandler _handler;

    public AddCharacterStateHandlerTests()
    {
        _mockNovelRepo = new Mock<INovelRepository>();
        _mockCharacterRepo = new Mock<ICharacterRepository>();
        _mockImageRepo = new Mock<IImageRepository>();
        _mockTransformMapper = new Mock<TransformDtoMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<CharacterStateDtoMapper>();
        
        _handler = new AddCharacterStateHandler(
            _mockNovelRepo.Object,
            _mockCharacterRepo.Object,
            _mockImageRepo.Object,
            _mockTransformMapper.Object,
            _mockUnitOfWork.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsCharacterState()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var characterId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        
        var novel = Novel.Create("Test Novel");
        var character = Character.Create("Alice", novelId, Domain.Common.Color.FromHex("FF5733"), null);
        var image = Image.CreatePending("test.png", novelId, "path/test.png", "png", ImageType.Character, new Size(512, 512));
        
        var command = new AddCharacterStateCommand 
        { 
            NovelId = novelId,
            CharacterId = characterId,
            Name = "happy",
            Description = "Happy expression",
            ImageId = imageId,
            LocalTransform = new TransformDto
            {
                X = 0.5,
                Y = 0.5,
                Width = 512,
                Height = 512,
                Scale = 1.0,
                Rotation = 0.0,
                ZIndex = 1
            }
        };
        
        var expectedDto = new CharacterStateDto(Guid.NewGuid(), "happy", "Happy expression", null, null);

        _mockNovelRepo
            .Setup(r => r.GetByIdAsync(novelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

        _mockCharacterRepo
            .Setup(r => r.GetByIdAsync(characterId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        _mockImageRepo
            .Setup(r => r.GetByIdAsync(imageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(image);

        _mockTransformMapper
            .Setup(m => m.ToDomainModel(It.IsAny<TransformDto>()))
            .Returns(Transform.Create(new Position(0.5, 0.5), new Size(512, 512), 1.0, 0.0, 1));

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
        Assert.Equal("happy", result.Name);
        _mockCharacterRepo.Verify(r => r.AddOrUpdateAsync(character, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingCharacter_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var characterId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        
        var novel = Novel.Create("Test Novel");
        
        var command = new AddCharacterStateCommand 
        { 
            NovelId = novelId,
            CharacterId = characterId,
            Name = "happy",
            ImageId = imageId,
            LocalTransform = new TransformDto
            {
                X = 0,
                Y = 0,
                Width = 512,
                Height = 512,
                Scale = 1,
                Rotation = 0,
                ZIndex = 0
            }
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
