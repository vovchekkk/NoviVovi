using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Images;

namespace NoviVovi.Infrastructure.Tests.Exporters.RenPy.Services.Images;

public class NovelImageCollectorTests
{
    private readonly NovelImageCollector _collector;

    public NovelImageCollectorTests()
    {
        _collector = new NovelImageCollector();
    }

    [Fact]
    public void CollectImages_WithShowBackgroundSteps_ReturnsBackgroundImages()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var image = Image.CreatePending("bg.png", Guid.NewGuid(), "/path/bg.png", "png", ImageType.Background, new Size(1920, 1080));
        typeof(Image).GetProperty(nameof(Image.Id))!.SetValue(image, imageId);

        var transform = Transform.Create(new Position(0, 0), new Size(1920, 1080));
        var backgroundObject = BackgroundObject.Create(image, transform);
        var step = ShowBackgroundStep.Create(backgroundObject);

        var novel = Novel.Create("Test Novel", "start");
        novel.StartLabel.AddStep(step);

        // Act
        var result = _collector.CollectImages(novel).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(imageId, result[0].ImageId);
        Assert.Equal("/path/bg.png", result[0].StoragePath);
        Assert.Equal($"bg_{imageId:N}", result[0].RenPyImageName);
    }

    [Fact]
    public void CollectImages_WithShowCharacterSteps_ReturnsCharacterImages()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var charId = Guid.NewGuid();
        var stateId = Guid.NewGuid();

        var image = Image.CreatePending("char.png", Guid.NewGuid(), "/path/char.png", "png", ImageType.Character, new Size(1920, 1080));
        typeof(Image).GetProperty(nameof(Image.Id))!.SetValue(image, imageId);

        var localTransform = Transform.Create(new Position(0, 0), new Size(500, 1000));
        var character = Character.Create("Alice", Guid.NewGuid(), Color.FromHex("#FF5733"), null);
        typeof(Character).GetProperty(nameof(Character.Id))!.SetValue(character, charId);

        var state = CharacterState.Create("happy", image, localTransform);
        typeof(CharacterState).GetProperty(nameof(CharacterState.Id))!.SetValue(state, stateId);

        var transform = Transform.Create(new Position(960, 540), new Size(500, 1000));
        var characterObject = CharacterObject.Create(character, state, transform);
        var step = ShowCharacterStep.Create(characterObject);

        var novel = Novel.Create("Test Novel", "start");
        novel.StartLabel.AddStep(step);

        // Act
        var result = _collector.CollectImages(novel).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(imageId, result[0].ImageId);
        Assert.Equal("/path/char.png", result[0].StoragePath);
        Assert.Equal($"char_{charId:N} state_{stateId:N}", result[0].RenPyImageName);
    }

    [Fact]
    public void CollectImages_WithDuplicateImages_ReturnsUniqueImages()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var image = Image.CreatePending("bg.png", Guid.NewGuid(), "/path/bg.png", "png", ImageType.Background, new Size(1920, 1080));
        typeof(Image).GetProperty(nameof(Image.Id))!.SetValue(image, imageId);

        var transform = Transform.Create(new Position(0, 0), new Size(1920, 1080));
        var backgroundObject = BackgroundObject.Create(image, transform);
        
        var step1 = ShowBackgroundStep.Create(backgroundObject);
        var step2 = ShowBackgroundStep.Create(backgroundObject);
        var step3 = ShowBackgroundStep.Create(backgroundObject);

        var novel = Novel.Create("Test Novel", "start");
        novel.StartLabel.AddStep(step1);
        novel.StartLabel.AddStep(step2);
        novel.StartLabel.AddStep(step3);

        // Act
        var result = _collector.CollectImages(novel).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(imageId, result[0].ImageId);
    }

    [Fact]
    public void CollectImages_WithEmptyNovel_ReturnsEmptyCollection()
    {
        // Arrange
        var novel = Novel.Create("Test Novel", "start");

        // Act
        var result = _collector.CollectImages(novel).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void CollectImages_WithMultipleLabels_CollectsFromAllLabels()
    {
        // Arrange
        var imageId1 = Guid.NewGuid();
        var imageId2 = Guid.NewGuid();

        var image1 = Image.CreatePending("bg1.png", Guid.NewGuid(), "/path/bg1.png", "png", ImageType.Background, new Size(1920, 1080));
        typeof(Image).GetProperty(nameof(Image.Id))!.SetValue(image1, imageId1);

        var image2 = Image.CreatePending("bg2.png", Guid.NewGuid(), "/path/bg2.png", "png", ImageType.Background, new Size(1920, 1080));
        typeof(Image).GetProperty(nameof(Image.Id))!.SetValue(image2, imageId2);

        var transform = Transform.Create(new Position(0, 0), new Size(1920, 1080));
        var backgroundObject1 = BackgroundObject.Create(image1, transform);
        var backgroundObject2 = BackgroundObject.Create(image2, transform);

        var step1 = ShowBackgroundStep.Create(backgroundObject1);
        var step2 = ShowBackgroundStep.Create(backgroundObject2);

        var novel = Novel.Create("Test Novel", "start");
        novel.StartLabel.AddStep(step1);

        var label2 = Label.Create("chapter2", novel.Id);
        novel.AddLabel(label2);
        label2.AddStep(step2);

        // Act
        var result = _collector.CollectImages(novel).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.ImageId == imageId1);
        Assert.Contains(result, r => r.ImageId == imageId2);
    }

    [Fact]
    public void CollectImages_WithMixedStepTypes_CollectsOnlyImageSteps()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var image = Image.CreatePending("bg.png", Guid.NewGuid(), "/path/bg.png", "png", ImageType.Background, new Size(1920, 1080));
        typeof(Image).GetProperty(nameof(Image.Id))!.SetValue(image, imageId);

        var transform = Transform.Create(new Position(0, 0), new Size(1920, 1080));
        var backgroundObject = BackgroundObject.Create(image, transform);
        var showBgStep = ShowBackgroundStep.Create(backgroundObject);
        
        // Use JumpStep instead of DialogueStep
        var novel = Novel.Create("Test Novel", "start");
        var jumpStep = JumpStep.Create(novel.StartLabel);

        novel.StartLabel.AddStep(jumpStep);
        novel.StartLabel.AddStep(showBgStep);

        // Act
        var result = _collector.CollectImages(novel).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(imageId, result[0].ImageId);
    }

    [Fact]
    public void CollectImages_WithBackgroundAndCharacter_ReturnsBothImages()
    {
        // Arrange
        var bgImageId = Guid.NewGuid();
        var charImageId = Guid.NewGuid();
        var charId = Guid.NewGuid();
        var stateId = Guid.NewGuid();

        var bgImage = Image.CreatePending("bg.png", Guid.NewGuid(), "/path/bg.png", "png", ImageType.Background, new Size(1920, 1080));
        typeof(Image).GetProperty(nameof(Image.Id))!.SetValue(bgImage, bgImageId);

        var charImage = Image.CreatePending("char.png", Guid.NewGuid(), "/path/char.png", "png", ImageType.Character, new Size(500, 1000));
        typeof(Image).GetProperty(nameof(Image.Id))!.SetValue(charImage, charImageId);

        var bgTransform = Transform.Create(new Position(0, 0), new Size(1920, 1080));
        var backgroundObject = BackgroundObject.Create(bgImage, bgTransform);

        var localTransform = Transform.Create(new Position(0, 0), new Size(500, 1000));
        var character = Character.Create("Alice", Guid.NewGuid(), Color.FromHex("#FF5733"), null);
        typeof(Character).GetProperty(nameof(Character.Id))!.SetValue(character, charId);

        var state = CharacterState.Create("happy", charImage, localTransform);
        typeof(CharacterState).GetProperty(nameof(CharacterState.Id))!.SetValue(state, stateId);

        var charTransform = Transform.Create(new Position(960, 540), new Size(500, 1000));
        var characterObject = CharacterObject.Create(character, state, charTransform);

        var bgStep = ShowBackgroundStep.Create(backgroundObject);
        var charStep = ShowCharacterStep.Create(characterObject);

        var novel = Novel.Create("Test Novel", "start");
        novel.StartLabel.AddStep(bgStep);
        novel.StartLabel.AddStep(charStep);

        // Act
        var result = _collector.CollectImages(novel).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.ImageId == bgImageId && r.RenPyImageName == $"bg_{bgImageId:N}");
        Assert.Contains(result, r => r.ImageId == charImageId && r.RenPyImageName == $"char_{charId:N} state_{stateId:N}");
    }

    [Fact]
    public void CollectImages_WithMultipleCharacterStates_CollectsAllUniqueStates()
    {
        // Arrange
        var imageId1 = Guid.NewGuid();
        var imageId2 = Guid.NewGuid();
        var charId = Guid.NewGuid();
        var stateId1 = Guid.NewGuid();
        var stateId2 = Guid.NewGuid();

        var image1 = Image.CreatePending("char_happy.png", Guid.NewGuid(), "/path/happy.png", "png", ImageType.Character, new Size(500, 1000));
        typeof(Image).GetProperty(nameof(Image.Id))!.SetValue(image1, imageId1);

        var image2 = Image.CreatePending("char_sad.png", Guid.NewGuid(), "/path/sad.png", "png", ImageType.Character, new Size(500, 1000));
        typeof(Image).GetProperty(nameof(Image.Id))!.SetValue(image2, imageId2);

        var localTransform = Transform.Create(new Position(0, 0), new Size(500, 1000));
        var character = Character.Create("Alice", Guid.NewGuid(), Color.FromHex("#FF5733"), null);
        typeof(Character).GetProperty(nameof(Character.Id))!.SetValue(character, charId);

        var state1 = CharacterState.Create("happy", image1, localTransform);
        typeof(CharacterState).GetProperty(nameof(CharacterState.Id))!.SetValue(state1, stateId1);

        var state2 = CharacterState.Create("sad", image2, localTransform);
        typeof(CharacterState).GetProperty(nameof(CharacterState.Id))!.SetValue(state2, stateId2);

        var transform = Transform.Create(new Position(960, 540), new Size(500, 1000));
        var characterObject1 = CharacterObject.Create(character, state1, transform);
        var characterObject2 = CharacterObject.Create(character, state2, transform);

        var step1 = ShowCharacterStep.Create(characterObject1);
        var step2 = ShowCharacterStep.Create(characterObject2);

        var novel = Novel.Create("Test Novel", "start");
        novel.StartLabel.AddStep(step1);
        novel.StartLabel.AddStep(step2);

        // Act
        var result = _collector.CollectImages(novel).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.ImageId == imageId1 && r.RenPyImageName == $"char_{charId:N} state_{stateId1:N}");
        Assert.Contains(result, r => r.ImageId == imageId2 && r.RenPyImageName == $"char_{charId:N} state_{stateId2:N}");
    }
}




