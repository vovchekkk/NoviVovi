using NoviVovi.Domain.Images;
using NoviVovi.Domain.Scene;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using NoviVovi.Infrastructure.Mappers;

namespace NoviVovi.Infrastructure.Tests.Mappers;

public class ImageMapperTests
{
    private readonly TransformMapper _transformMapper = new();
    private readonly ImageMapper _mapper;

    public ImageMapperTests()
    {
        _mapper = new ImageMapper(_transformMapper);
    }

    [Fact]
    public void ToDomain_ValidImageDbO_ReturnsImage()
    {
        // Arrange
        var dbo = new ImageDbO
        {
            Id = Guid.NewGuid(),
            Name = "test.png",
            NovelId = Guid.NewGuid(),
            Url = "https://example.com/test.png",
            Format = "png",
            ImgType = "background",
            Width = 1920,
            Height = 1080
        };

        // Act
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dbo.Id, result.Id);
        Assert.Equal(dbo.Name, result.Name);
        Assert.Equal(dbo.NovelId, result.NovelId);
        Assert.Equal(dbo.Url, result.StoragePath);
        Assert.Equal(dbo.Format, result.Format);
        Assert.Equal(ImageType.Background, result.Type);
        Assert.Equal(1920, result.Size.Width);
        Assert.Equal(1080, result.Size.Height);
        Assert.Equal(ImageStatus.Active, result.Status);
    }

    [Fact]
    public void ToDbO_ValidImage_ReturnsImageDbO()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var image = new Image(
            Guid.NewGuid(),
            "test.png",
            novelId,
            "https://example.com/test.png",
            "png",
            ImageType.Character,
            new Size(512, 512),
            ImageStatus.Active
        );

        // Act
        var result = _mapper.ToDbO(image);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(image.Id, result.Id);
        Assert.Equal(image.Name, result.Name);
        Assert.Equal(image.NovelId, result.NovelId);
        Assert.Equal(image.StoragePath, result.Url);
        Assert.Equal(image.Format, result.Format);
        Assert.Equal("character", result.ImgType);
        Assert.Equal(512, result.Width);
        Assert.Equal(512, result.Height);
    }

    [Fact]
    public void RoundTrip_Image_PreservesAllValues()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var original = new Image(
            Guid.NewGuid(),
            "background.jpg",
            novelId,
            "https://cdn.example.com/bg.jpg",
            "jpg",
            ImageType.Background,
            new Size(1920, 1080),
            ImageStatus.Active
        );

        // Act
        var dbo = _mapper.ToDbO(original);
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.Equal(original.Id, result.Id);
        Assert.Equal(original.Name, result.Name);
        Assert.Equal(original.NovelId, result.NovelId);
        Assert.Equal(original.StoragePath, result.StoragePath);
        Assert.Equal(original.Format, result.Format);
        Assert.Equal(original.Type, result.Type);
        Assert.Equal(original.Size.Width, result.Size.Width);
        Assert.Equal(original.Size.Height, result.Size.Height);
    }

    [Fact]
    public void ToDomain_BackgroundDbO_ReturnsBackgroundObject()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var transformId = Guid.NewGuid();
        var backgroundId = Guid.NewGuid();

        var dbo = new BackgroundDbO
        {
            Id = backgroundId,
            Img = imageId,
            TransformId = transformId,
            Image = new ImageDbO
            {
                Id = imageId,
                Name = "bg.png",
                NovelId = Guid.NewGuid(),
                Url = "https://example.com/bg.png",
                Format = "png",
                ImgType = "background",
                Width = 1920,
                Height = 1080
            },
            Transform = new TransformDbO
            {
                Id = transformId,
                Width = 1920,
                Height = 1080,
                Scale = 1.0m,
                Rotation = 0m,
                XPos = 0m,
                YPos = 0m,
                ZIndex = 0
            }
        };

        // Act
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(backgroundId, result.Id);
        Assert.NotNull(result.Image);
        Assert.Equal(imageId, result.Image.Id);
        Assert.NotNull(result.Transform);
        Assert.Equal(transformId, result.Transform.Id);
    }

    [Fact]
    public void ToDomain_BackgroundDbO_WithoutImage_ThrowsArgumentException()
    {
        // Arrange
        var dbo = new BackgroundDbO
        {
            Id = Guid.NewGuid(),
            Img = Guid.NewGuid(),
            TransformId = Guid.NewGuid(),
            Image = null,
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
        Assert.Equal("Bg object should have image", exception.Message);
    }

    [Fact]
    public void ToDbO_BackgroundObject_ReturnsBackgroundDbO()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var image = new Image(
            Guid.NewGuid(),
            "bg.png",
            novelId,
            "https://example.com/bg.png",
            "png",
            ImageType.Background,
            new Size(1920, 1080),
            ImageStatus.Active
        );

        var transform = new Transform(
            Guid.NewGuid(),
            new Position(0, 0),
            new Size(1920, 1080),
            1.0,
            0.0,
            0
        );

        var background = new BackgroundObject(
            Guid.NewGuid(),
            image,
            transform
        );

        // Act
        var result = _mapper.ToDbO(background, novelId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(background.Id, result.Id);
        Assert.Equal(image.Id, result.Img);
        Assert.Equal(transform.Id, result.TransformId);
        Assert.NotNull(result.Image);
        Assert.NotNull(result.Transform);
    }

    [Fact]
    public void RoundTrip_BackgroundObject_PreservesAllValues()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var image = new Image(
            Guid.NewGuid(),
            "bg.png",
            novelId,
            "https://example.com/bg.png",
            "png",
            ImageType.Background,
            new Size(1920, 1080),
            ImageStatus.Active
        );

        var transform = new Transform(
            Guid.NewGuid(),
            new Position(10, 20),
            new Size(1920, 1080),
            1.5,
            45.0,
            1
        );

        var original = new BackgroundObject(
            Guid.NewGuid(),
            image,
            transform
        );

        // Act
        var dbo = _mapper.ToDbO(original, novelId);
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.Equal(original.Id, result.Id);
        Assert.Equal(original.Image.Id, result.Image.Id);
        Assert.Equal(original.Transform.Id, result.Transform.Id);
        Assert.Equal(original.Transform.Position.X, result.Transform.Position.X);
        Assert.Equal(original.Transform.Position.Y, result.Transform.Position.Y);
        Assert.Equal(original.Transform.Scale, result.Transform.Scale);
        Assert.Equal(original.Transform.Rotation, result.Transform.Rotation);
        Assert.Equal(original.Transform.ZIndex, result.Transform.ZIndex);
    }
}
