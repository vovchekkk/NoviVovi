using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Scene;
using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using NoviVovi.Infrastructure.Mappers;

namespace NoviVovi.Infrastructure.Tests.Mappers;

public class CharacterMapperTests
{
    private readonly TransformMapper _transformMapper = new();
    private readonly ImageMapper _imageMapper;
    private readonly CharacterMapper _mapper;

    public CharacterMapperTests()
    {
        _imageMapper = new ImageMapper(_transformMapper);
        _mapper = new CharacterMapper(_imageMapper, _transformMapper);
    }

    [Fact]
    public void ToDomain_ValidCharacterDbO_ReturnsCharacter()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var novelId = Guid.NewGuid();
        
        var dbo = new CharacterDbO
        {
            Id = characterId,
            Name = "Alice",
            NovelId = novelId,
            NameColor = "FF5733",
            Description = "Main character",
            States = []
        };

        // Act
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(characterId, result.Id);
        Assert.Equal("Alice", result.Name);
        Assert.Equal(novelId, result.NovelId);
        Assert.Equal("#FF5733", result.NameColor.ToString());
        Assert.Equal("Main character", result.Description);
        Assert.Empty(result.CharacterStates);
    }

    [Fact]
    public void ToDomain_CharacterDbO_WithStates_ReturnsCharacterWithStates()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var stateId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        
        var dbo = new CharacterDbO
        {
            Id = characterId,
            Name = "Bob",
            NovelId = Guid.NewGuid(),
            NameColor = "00FF00",
            Description = null,
            States =
            [
                new CharacterStateDbO
                {
                    Id = stateId,
                    CharacterId = characterId,
                    StateName = "happy",
                    Description = "Happy state",
                    ImageId = imageId,
                    Image = new ImageDbO
                    {
                        Id = imageId,
                        Name = "bob_happy.png",
                        NovelId = Guid.NewGuid(),
                        Url = "https://example.com/bob_happy.png",
                        Format = "png",
                        ImgType = "character",
                        Width = 512,
                        Height = 512
                    },
                    Transform = new TransformDbO
                    {
                        Id = Guid.NewGuid(),
                        Width = 512,
                        Height = 512,
                        Scale = 1m,
                        Rotation = 0m,
                        XPos = 0m,
                        YPos = 0m,
                        ZIndex = 0
                    }
                }
            ]
        };

        // Act
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.CharacterStates);
        var state = result.CharacterStates.First();
        Assert.Equal(stateId, state.Id);
        Assert.Equal("happy", state.Name);
        Assert.Equal("Happy state", state.Description);
    }

    [Fact]
    public void ToDbO_ValidCharacter_ReturnsCharacterDbO()
    {
        // Arrange
        var character = new Character(
            Guid.NewGuid(),
            "Charlie",
            Guid.NewGuid(),
            Color.FromHex("0000FF"),
            "Side character"
        );

        // Act
        var result = _mapper.ToDbO(character);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(character.Id, result.Id);
        Assert.Equal("Charlie", result.Name);
        Assert.Equal(character.NovelId, result.NovelId);
        Assert.Equal("0000FF", result.NameColor);
        Assert.Equal("Side character", result.Description);
        Assert.Empty(result.States);
    }

    [Fact]
    public void ToDbO_CharacterWithStates_ReturnsCharacterDbOWithStates()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var character = new Character(
            Guid.NewGuid(),
            "Diana",
            novelId,
            Color.FromHex("FF00FF"),
            null
        );

        var image = new Image(
            Guid.NewGuid(),
            "diana_sad.png",
            novelId,
            "https://example.com/diana_sad.png",
            "png",
            ImageType.Character,
            new Size(512, 512),
            ImageStatus.Active
        );

        var transform = new Transform(
            Guid.NewGuid(),
            new Position(0, 0),
            new Size(512, 512),
            1.0,
            0.0,
            0
        );

        var state = new CharacterState(
            Guid.NewGuid(),
            "sad",
            image,
            transform,
            "Sad expression"
        );

        character.AddCharacterState(state);

        // Act
        var result = _mapper.ToDbO(character);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.States);
        var stateDbO = result.States.First();
        Assert.Equal(state.Id, stateDbO.Id);
        Assert.Equal("sad", stateDbO.StateName);
        Assert.Equal("Sad expression", stateDbO.Description);
        Assert.Equal(character.Id, stateDbO.CharacterId);
    }

    [Fact]
    public void RoundTrip_Character_PreservesAllValues()
    {
        // Arrange
        var original = new Character(
            Guid.NewGuid(),
            "Eve",
            Guid.NewGuid(),
            Color.FromHex("FFFF00"),
            "Mysterious character"
        );

        // Act
        var dbo = _mapper.ToDbO(original);
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.Equal(original.Id, result.Id);
        Assert.Equal(original.Name, result.Name);
        Assert.Equal(original.NovelId, result.NovelId);
        Assert.Equal(original.NameColor.ToString(), result.NameColor.ToString());
        Assert.Equal(original.Description, result.Description);
    }

    [Fact]
    public void ToDomain_CharacterStateDbO_ReturnsCharacterState()
    {
        // Arrange
        var stateId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var transformId = Guid.NewGuid();
        
        var dbo = new CharacterStateDbO
        {
            Id = stateId,
            CharacterId = Guid.NewGuid(),
            StateName = "angry",
            Description = "Angry expression",
            ImageId = imageId,
            Image = new ImageDbO
            {
                Id = imageId,
                Name = "char_angry.png",
                NovelId = Guid.NewGuid(),
                Url = "https://example.com/angry.png",
                Format = "png",
                ImgType = "character",
                Width = 512,
                Height = 512
            },
            Transform = new TransformDbO
            {
                Id = transformId,
                Width = 512,
                Height = 512,
                Scale = 1.2m,
                Rotation = 5m,
                XPos = 10m,
                YPos = 20m,
                ZIndex = 1
            }
        };

        // Act
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(stateId, result.Id);
        Assert.Equal("angry", result.Name);
        Assert.Equal("Angry expression", result.Description);
        Assert.NotNull(result.Image);
        Assert.Equal(imageId, result.Image.Id);
        Assert.NotNull(result.LocalTransform);
        Assert.Equal(transformId, result.LocalTransform.Id);
    }

    [Fact]
    public void ToDbO_CharacterObject_ReturnsStepCharacterDbO()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var character = new Character(
            Guid.NewGuid(),
            "Frank",
            novelId,
            Color.FromHex("00FFFF"),
            null
        );

        var image = new Image(
            Guid.NewGuid(),
            "frank.png",
            novelId,
            "https://example.com/frank.png",
            "png",
            ImageType.Character,
            new Size(512, 512),
            ImageStatus.Active
        );

        var stateTransform = new Transform(
            Guid.NewGuid(),
            new Position(0, 0),
            new Size(512, 512),
            1.0,
            0.0,
            0
        );

        var state = new CharacterState(
            Guid.NewGuid(),
            "neutral",
            image,
            stateTransform,
            null
        );

        var objectTransform = new Transform(
            Guid.NewGuid(),
            new Position(100, 200),
            new Size(512, 512),
            1.5,
            10.0,
            2
        );

        var characterObject = new CharacterObject(
            Guid.NewGuid(),
            character,
            state,
            objectTransform
        );

        // Act
        var result = _mapper.ToDbO(characterObject);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(characterObject.Id, result.Id);
        Assert.Equal(state.Id, result.CharacterStateId);
        Assert.Equal(objectTransform.Id, result.TransformId);
        Assert.NotNull(result.Character);
        Assert.NotNull(result.CharacterState);
        Assert.NotNull(result.Transform);
    }

    [Fact]
    public void ToDomain_StepCharacterDbO_ReturnsCharacterObject()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var stateId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var transformId = Guid.NewGuid();
        var objectId = Guid.NewGuid();

        var dbo = new StepCharacterDbO
        {
            Id = objectId,
            CharacterStateId = stateId,
            TransformId = transformId,
            Character = new CharacterDbO
            {
                Id = characterId,
                Name = "Grace",
                NovelId = Guid.NewGuid(),
                NameColor = "FF0000",
                Description = null,
                States = []
            },
            CharacterState = new CharacterStateDbO
            {
                Id = stateId,
                CharacterId = characterId,
                StateName = "surprised",
                Description = null,
                ImageId = imageId,
                Image = new ImageDbO
                {
                    Id = imageId,
                    Name = "grace_surprised.png",
                    NovelId = Guid.NewGuid(),
                    Url = "https://example.com/surprised.png",
                    Format = "png",
                    ImgType = "character",
                    Width = 512,
                    Height = 512
                },
                Transform = new TransformDbO
                {
                    Id = Guid.NewGuid(),
                    Width = 512,
                    Height = 512,
                    Scale = 1m,
                    Rotation = 0m,
                    XPos = 0m,
                    YPos = 0m,
                    ZIndex = 0
                }
            },
            Transform = new TransformDbO
            {
                Id = transformId,
                Width = 512,
                Height = 512,
                Scale = 1.3m,
                Rotation = 15m,
                XPos = 50m,
                YPos = 100m,
                ZIndex = 3
            }
        };

        // Act
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(objectId, result.Id);
        Assert.NotNull(result.Character);
        Assert.Equal(characterId, result.Character.Id);
        Assert.NotNull(result.State);
        Assert.Equal(stateId, result.State.Id);
        Assert.NotNull(result.Transform);
        Assert.Equal(transformId, result.Transform.Id);
    }

    [Fact]
    public void ToDomain_StepCharacterDbO_WithoutCharacterState_ThrowsArgumentException()
    {
        // Arrange
        var dbo = new StepCharacterDbO
        {
            Id = Guid.NewGuid(),
            CharacterStateId = Guid.NewGuid(),
            TransformId = Guid.NewGuid(),
            Character = new CharacterDbO
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                NovelId = Guid.NewGuid(),
                NameColor = "FFFFFF",
                Description = null,
                States = []
            },
            CharacterState = null,
            Transform = new TransformDbO
            {
                Id = Guid.NewGuid(),
                Width = 100,
                Height = 100,
                Scale = 1m,
                Rotation = 0m,
                XPos = 0m,
                YPos = 0m,
                ZIndex = 0
            }
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _mapper.ToDomain(dbo));
        Assert.Equal("Invalid step character", exception.Message);
    }

    [Fact]
    public void ToDomain_StepCharacterDbO_WithoutTransform_ThrowsArgumentException()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var dbo = new StepCharacterDbO
        {
            Id = Guid.NewGuid(),
            CharacterStateId = Guid.NewGuid(),
            TransformId = Guid.NewGuid(),
            Character = new CharacterDbO
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                NovelId = Guid.NewGuid(),
                NameColor = "FFFFFF",
                Description = null,
                States = []
            },
            CharacterState = new CharacterStateDbO
            {
                Id = Guid.NewGuid(),
                CharacterId = Guid.NewGuid(),
                StateName = "test",
                Description = null,
                ImageId = imageId,
                Image = new ImageDbO
                {
                    Id = imageId,
                    Name = "test.png",
                    NovelId = Guid.NewGuid(),
                    Url = "https://example.com/test.png",
                    Format = "png",
                    ImgType = "character",
                    Width = 100,
                    Height = 100
                },
                Transform = new TransformDbO
                {
                    Id = Guid.NewGuid(),
                    Width = 100,
                    Height = 100,
                    Scale = 1m,
                    Rotation = 0m,
                    XPos = 0m,
                    YPos = 0m,
                    ZIndex = 0
                }
            },
            Transform = null
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _mapper.ToDomain(dbo));
        Assert.Equal("Invalid step character", exception.Message);
    }
}
