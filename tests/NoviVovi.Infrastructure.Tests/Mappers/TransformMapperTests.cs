using NoviVovi.Domain.Scene;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using NoviVovi.Infrastructure.Mappers;

namespace NoviVovi.Infrastructure.Tests.Mappers;

public class TransformMapperTests
{
    private readonly TransformMapper _mapper = new();

    [Fact]
    public void ToDbO_WithRotationZero_ShouldNotSetRotationToNull()
    {
        // Arrange
        var transform = new Transform(
            Guid.NewGuid(),
            new Position(10, 20),
            new Size(100, 200),
            scale: 1.0,
            rotation: 0.0,
            zIndex: 0
        );

        // Act
        var dbo = _mapper.ToDbO(transform);

        // Assert
        Assert.NotNull(dbo);
        Assert.Equal(0m, dbo.Rotation);
        Assert.NotNull(dbo.Rotation);
    }

    [Fact]
    public void ToDbO_WithRotationNonZero_ShouldSetCorrectRotation()
    {
        // Arrange
        var transform = new Transform(
            Guid.NewGuid(),
            new Position(10, 20),
            new Size(100, 200),
            scale: 1.0,
            rotation: 45.0,
            zIndex: 0
        );

        // Act
        var dbo = _mapper.ToDbO(transform);

        // Assert
        Assert.NotNull(dbo);
        Assert.Equal(45m, dbo.Rotation);
    }

    [Fact]
    public void ToDbO_WithScaleZero_ShouldSetScaleToZero()
    {
        // Arrange
        var transform = new Transform(
            Guid.NewGuid(),
            new Position(10, 20),
            new Size(100, 200),
            scale: 0.0,
            rotation: 45.0,
            zIndex: 0
        );

        // Act
        var dbo = _mapper.ToDbO(transform);

        // Assert
        Assert.NotNull(dbo);
        Assert.Equal(0m, dbo.Scale);
        Assert.Equal(45m, dbo.Rotation);
    }

    [Fact]
    public void ToDbO_WithAllZeroValues_ShouldNotSetNullValues()
    {
        // Arrange
        var transform = new Transform(
            Guid.NewGuid(),
            new Position(0, 0),
            new Size(100, 200),
            scale: 0.0,
            rotation: 0.0,
            zIndex: 0
        );

        // Act
        var dbo = _mapper.ToDbO(transform);

        // Assert
        Assert.NotNull(dbo);
        Assert.Equal(0m, dbo.Scale);
        Assert.Equal(0m, dbo.Rotation);
        Assert.Equal(0m, dbo.XPos);
        Assert.Equal(0m, dbo.YPos);
        Assert.Equal(0, dbo.ZIndex);
        Assert.NotNull(dbo.Rotation);
        Assert.NotNull(dbo.Scale);
    }

    [Fact]
    public void ToDomain_WithNullRotation_ShouldDefaultToZero()
    {
        // Arrange
        var dbo = new TransformDbO
        {
            Id = Guid.NewGuid(),
            Width = 100,
            Height = 200,
            Scale = 1m,
            Rotation = null,
            XPos = 10m,
            YPos = 20m,
            ZIndex = 0
        };

        // Act
        var domain = _mapper.ToDomain(dbo);

        // Assert
        Assert.NotNull(domain);
        Assert.Equal(0.0, domain.Rotation);
    }

    [Fact]
    public void ToDomain_WithNullScale_ShouldDefaultToOne()
    {
        // Arrange
        var dbo = new TransformDbO
        {
            Id = Guid.NewGuid(),
            Width = 100,
            Height = 200,
            Scale = null,
            Rotation = 45m,
            XPos = 10m,
            YPos = 20m,
            ZIndex = 0
        };

        // Act
        var domain = _mapper.ToDomain(dbo);

        // Assert
        Assert.NotNull(domain);
        Assert.Equal(1.0, domain.Scale);
    }

    [Fact]
    public void RoundTrip_ShouldPreserveAllValues()
    {
        // Arrange
        var original = new Transform(
            Guid.NewGuid(),
            new Position(15.5, 25.5),
            new Size(150, 250),
            scale: 1.5,
            rotation: 90.0,
            zIndex: 5
        );

        // Act
        var dbo = _mapper.ToDbO(original);
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.Equal(original.Id, result.Id);
        Assert.Equal(original.Position.X, result.Position.X);
        Assert.Equal(original.Position.Y, result.Position.Y);
        Assert.Equal(original.Size.Width, result.Size.Width);
        Assert.Equal(original.Size.Height, result.Size.Height);
        Assert.Equal(original.Scale, result.Scale);
        Assert.Equal(original.Rotation, result.Rotation);
        Assert.Equal(original.ZIndex, result.ZIndex);
    }
}
