using NoviVovi.Domain.Scene;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Mappers;

namespace NoviVovi.Infrastructure.Tests.Exporters.RenPy.Mappers;

public class TransformToRenPyMapperTests
{
    private readonly TransformToRenPyMapper _mapper;

    public TransformToRenPyMapperTests()
    {
        _mapper = new TransformToRenPyMapper();
    }

    [Fact]
    public void Map_ValidTransform_ReturnsRenPyTransform()
    {
        // Arrange
        var transform = Transform.Create(
            new Position(100.5, 200.7),
            new Size(800, 600),
            scale: 1.5,
            rotation: 45.0,
            zIndex: 10
        );

        // Act
        var result = _mapper.Map(transform);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100.5, result.XPos, precision: 1);
        Assert.Equal(200.7, result.YPos, precision: 1);
        Assert.Equal(1.5, result.Zoom);
        Assert.Equal(1.0, result.XZoom); // Default when no image size provided
        Assert.Equal(1.0, result.YZoom); // Default when no image size provided
        Assert.Equal(45.0, result.Rotate);
        Assert.Equal(10, result.ZOrder);
    }

    [Fact]
    public void Map_TransformWithDefaultValues_MapsCorrectly()
    {
        // Arrange
        var transform = Transform.Create(
            new Position(0, 0),
            new Size(100, 100)
        );

        // Act
        var result = _mapper.Map(transform);

        // Assert
        Assert.Equal(0.0, result.XPos);
        Assert.Equal(0.0, result.YPos);
        Assert.Equal(1.0, result.Zoom);
        Assert.Equal(1.0, result.XZoom);
        Assert.Equal(1.0, result.YZoom);
        Assert.Equal(0.0, result.Rotate);
        Assert.Equal(0, result.ZOrder);
    }

    [Fact]
    public void Map_TransformWithNegativePosition_MapsCorrectly()
    {
        // Arrange
        var transform = Transform.Create(
            new Position(-50.8, -100.2),
            new Size(200, 300),
            scale: 0.5,
            rotation: -90.0,
            zIndex: -5
        );

        // Act
        var result = _mapper.Map(transform);

        // Assert
        Assert.Equal(-50.8, result.XPos, precision: 1);
        Assert.Equal(-100.2, result.YPos, precision: 1);
        Assert.Equal(0.5, result.Zoom);
        Assert.Equal(1.0, result.XZoom);
        Assert.Equal(1.0, result.YZoom);
        Assert.Equal(-90.0, result.Rotate);
        Assert.Equal(-5, result.ZOrder);
    }

    [Fact]
    public void Map_TransformWithDecimalPositions_PreservesDecimals()
    {
        // Arrange
        var transform = Transform.Create(
            new Position(123.9, 456.1),
            new Size(100, 100)
        );

        // Act
        var result = _mapper.Map(transform);

        // Assert
        Assert.Equal(123.9, result.XPos, precision: 1);
        Assert.Equal(456.1, result.YPos, precision: 1);
        Assert.Equal(1.0, result.XZoom);
        Assert.Equal(1.0, result.YZoom);
    }

    [Fact]
    public void Map_TransformWithLargeValues_MapsCorrectly()
    {
        // Arrange
        var transform = Transform.Create(
            new Position(10000.0, 20000.0),
            new Size(5000, 5000),
            scale: 10.0,
            rotation: 360.0,
            zIndex: 1000
        );

        // Act
        var result = _mapper.Map(transform);

        // Assert
        Assert.Equal(10000.0, result.XPos);
        Assert.Equal(20000.0, result.YPos);
        Assert.Equal(10.0, result.Zoom);
        Assert.Equal(1.0, result.XZoom);
        Assert.Equal(1.0, result.YZoom);
        Assert.Equal(360.0, result.Rotate);
        Assert.Equal(1000, result.ZOrder);
    }

    [Fact]
    public void Map_TransformWithImageSize_CalculatesXZoomAndYZoom()
    {
        // Arrange
        var transform = Transform.Create(
            new Position(0, 0),
            new Size(800, 600) // Transform size
        );
        var imageSize = new Size(400, 300); // Original image size

        // Act
        var result = _mapper.Map(transform, imageSize);

        // Assert
        Assert.Equal(2.0, result.XZoom); // 800 / 400 = 2.0
        Assert.Equal(2.0, result.YZoom); // 600 / 300 = 2.0
    }

    [Fact]
    public void Map_TransformWithDifferentAspectRatio_CalculatesIndependentZoom()
    {
        // Arrange
        var transform = Transform.Create(
            new Position(0, 0),
            new Size(1600, 600) // Transform size (wide)
        );
        var imageSize = new Size(400, 300); // Original image size

        // Act
        var result = _mapper.Map(transform, imageSize);

        // Assert
        Assert.Equal(4.0, result.XZoom); // 1600 / 400 = 4.0
        Assert.Equal(2.0, result.YZoom); // 600 / 300 = 2.0
    }

    [Fact]
    public void Map_TransformWithZeroImageSize_ReturnsDefaultZoom()
    {
        // Arrange
        var transform = Transform.Create(
            new Position(0, 0),
            new Size(800, 600)
        );
        var imageSize = new Size(0, 0); // Invalid image size

        // Act
        var result = _mapper.Map(transform, imageSize);

        // Assert
        Assert.Equal(1.0, result.XZoom); // Default to 1.0 when image size is 0
        Assert.Equal(1.0, result.YZoom); // Default to 1.0 when image size is 0
    }

    [Fact]
    public void Map_TransformWithZeroScale_MapsCorrectly()
    {
        // Arrange
        var transform = Transform.Create(
            new Position(50, 50),
            new Size(100, 100),
            scale: 0.0
        );

        // Act
        var result = _mapper.Map(transform);

        // Assert
        Assert.Equal(0.0, result.Zoom);
    }

    [Fact]
    public void Map_TransformWithVerySmallScale_MapsCorrectly()
    {
        // Arrange
        var transform = Transform.Create(
            new Position(100, 100),
            new Size(100, 100),
            scale: 0.001
        );

        // Act
        var result = _mapper.Map(transform);

        // Assert
        Assert.Equal(0.001, result.Zoom);
    }

    [Fact]
    public void Map_TransformWithFullRotation_MapsCorrectly()
    {
        // Arrange
        var transform = Transform.Create(
            new Position(100, 100),
            new Size(100, 100),
            rotation: 720.0
        );

        // Act
        var result = _mapper.Map(transform);

        // Assert
        Assert.Equal(720.0, result.Rotate);
    }

    [Fact]
    public void Map_MultipleTransforms_EachMapsIndependently()
    {
        // Arrange
        var transform1 = Transform.Create(new Position(10, 20), new Size(100, 100), scale: 1.0);
        var transform2 = Transform.Create(new Position(30, 40), new Size(200, 200), scale: 2.0);

        // Act
        var result1 = _mapper.Map(transform1);
        var result2 = _mapper.Map(transform2);

        // Assert
        Assert.Equal(10, result1.XPos);
        Assert.Equal(20, result1.YPos);
        Assert.Equal(30, result2.XPos);
        Assert.Equal(40, result2.YPos);
        Assert.NotEqual(result1.Zoom, result2.Zoom);
    }
}
