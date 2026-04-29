using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Dialogue;
using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.Mappers;

namespace NoviVovi.Infrastructure.Tests.Mappers;

public class ReplicaMapperTests
{
    private readonly TransformMapper _transformMapper = new();
    private readonly ImageMapper _imageMapper;
    private readonly CharacterMapper _characterMapper;
    private readonly ReplicaMapper _mapper;

    public ReplicaMapperTests()
    {
        _imageMapper = new ImageMapper(_transformMapper);
        _characterMapper = new CharacterMapper(_imageMapper, _transformMapper);
        _mapper = new ReplicaMapper(_characterMapper);
    }

    [Fact]
    public void ToDomain_ValidReplicaDbO_ReturnsReplica()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var replicaId = Guid.NewGuid();
        
        var dbo = new ReplicaDbO
        {
            Id = replicaId,
            SpeakerId = characterId,
            Text = "Hello, world!",
            Speaker = new CharacterDbO
            {
                Id = characterId,
                Name = "TestChar",
                NovelId = Guid.NewGuid(),
                NameColor = "FF5733",
                Description = "Test character",
                States = []
            }
        };

        // Act
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(replicaId, result.Id);
        Assert.Equal("Hello, world!", result.Text);
        Assert.NotNull(result.Speaker);
        Assert.Equal(characterId, result.Speaker.Id);
        Assert.Equal("TestChar", result.Speaker.Name);
    }

    [Fact]
    public void ToDomain_ReplicaDbO_WithoutSpeaker_ThrowsArgumentException()
    {
        // Arrange
        var dbo = new ReplicaDbO
        {
            Id = Guid.NewGuid(),
            SpeakerId = Guid.NewGuid(),
            Text = "Hello!",
            Speaker = null
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _mapper.ToDomain(dbo));
        Assert.Equal("Incorrect replica", exception.Message);
    }

    [Fact]
    public void ToDomain_ReplicaDbO_WithoutText_ThrowsArgumentException()
    {
        // Arrange
        var dbo = new ReplicaDbO
        {
            Id = Guid.NewGuid(),
            SpeakerId = Guid.NewGuid(),
            Text = null,
            Speaker = new CharacterDbO
            {
                Id = Guid.NewGuid(),
                Name = "TestChar",
                NovelId = Guid.NewGuid(),
                NameColor = "FF5733",
                Description = null,
                States = []
            }
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _mapper.ToDomain(dbo));
        Assert.Equal("Incorrect replica", exception.Message);
    }

    [Fact]
    public void ToDbO_ValidReplica_ReturnsReplicaDbO()
    {
        // Arrange
        var character = new Character(
            Guid.NewGuid(),
            "TestChar",
            Guid.NewGuid(),
            Color.FromHex("FF5733"),
            "Test description"
        );

        var replica = new Replica(
            Guid.NewGuid(),
            character,
            "This is a test replica"
        );

        // Act
        var result = _mapper.ToDbO(replica);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(replica.Id, result.Id);
        Assert.Equal(replica.Text, result.Text);
        Assert.Equal(character.Id, result.SpeakerId);
        Assert.NotNull(result.Speaker);
        Assert.Equal(character.Id, result.Speaker.Id);
        Assert.Equal(character.Name, result.Speaker.Name);
    }

    [Fact]
    public void RoundTrip_Replica_PreservesAllValues()
    {
        // Arrange
        var character = new Character(
            Guid.NewGuid(),
            "Alice",
            Guid.NewGuid(),
            Color.FromHex("00FF00"),
            "Main character"
        );

        var original = new Replica(
            Guid.NewGuid(),
            character,
            "I have something important to say!"
        );

        // Act
        var dbo = _mapper.ToDbO(original);
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.Equal(original.Id, result.Id);
        Assert.Equal(original.Text, result.Text);
        Assert.Equal(original.Speaker.Id, result.Speaker.Id);
        Assert.Equal(original.Speaker.Name, result.Speaker.Name);
        Assert.Equal(original.Speaker.NameColor.ToString(), result.Speaker.NameColor.ToString());
    }

    [Fact]
    public void ToDbO_ReplicaWithLongText_PreservesText()
    {
        // Arrange
        var character = new Character(
            Guid.NewGuid(),
            "Narrator",
            Guid.NewGuid(),
            Color.FromHex("FFFFFF"),
            null
        );

        var longText = new string('A', 5000);
        var replica = new Replica(
            Guid.NewGuid(),
            character,
            longText
        );

        // Act
        var result = _mapper.ToDbO(replica);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(longText, result.Text);
        Assert.Equal(5000, result.Text.Length);
    }

    [Fact]
    public void ToDbO_ReplicaWithSpecialCharacters_PreservesText()
    {
        // Arrange
        var character = new Character(
            Guid.NewGuid(),
            "TestChar",
            Guid.NewGuid(),
            Color.FromHex("FF5733"),
            null
        );

        var specialText = "Hello! \"Quotes\" and 'apostrophes' and \n newlines \t tabs";
        var replica = new Replica(
            Guid.NewGuid(),
            character,
            specialText
        );

        // Act
        var result = _mapper.ToDbO(replica);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(specialText, result.Text);
    }
}
