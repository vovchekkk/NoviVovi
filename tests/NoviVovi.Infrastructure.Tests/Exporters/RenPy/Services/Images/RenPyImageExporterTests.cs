using System.IO.Compression;
using System.Text;
using Moq;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Images;

namespace NoviVovi.Infrastructure.Tests.Exporters.RenPy.Services.Images;

public class RenPyImageExporterTests
{
    private readonly Mock<IStorageService> _mockStorageService;
    private readonly RenPyImageExporter _exporter;

    public RenPyImageExporterTests()
    {
        _mockStorageService = new Mock<IStorageService>();
        _exporter = new RenPyImageExporter(_mockStorageService.Object);
    }

    [Fact]
    public async Task ExportAsync_WithSingleImage_CreatesCorrectZipEntry()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var storagePath = $"novels/images/{imageId}.png";
        var images = new List<ImageExportInfo>
        {
            new(imageId, storagePath, $"bg_{imageId:N}")
        };

        var imageData = "fake image data"u8.ToArray();
        var imageStream = new MemoryStream(imageData);

        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(storagePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(imageStream);

        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        // Act
        await _exporter.ExportAsync(archive, images, CancellationToken.None);
        archive.Dispose();

        // Assert
        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        var entry = readArchive.GetEntry($"game/images/bg_{imageId:N}.png");
        
        Assert.NotNull(entry);
        
        using var entryStream = entry.Open();
        using var reader = new StreamReader(entryStream);
        var content = await reader.ReadToEndAsync();
        
        Assert.Equal("fake image data", content);
    }

    [Fact]
    public async Task ExportAsync_WithRenPyNaming_UsesCorrectFileNames()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var storagePath = $"novels/images/{imageId}.png";
        var images = new List<ImageExportInfo>
        {
            new(imageId, storagePath, "bg_room")
        };

        var imageStream = new MemoryStream("data"u8.ToArray());
        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(storagePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(imageStream);

        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        // Act
        await _exporter.ExportAsync(archive, images, CancellationToken.None);
        archive.Dispose();

        // Assert
        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        var entry = readArchive.GetEntry("game/images/bg_room.png");
        
        Assert.NotNull(entry);
        Assert.Equal("game/images/bg_room.png", entry.FullName);
    }

    [Fact]
    public async Task ExportAsync_WithCharacterImage_UsesCorrectNamingConvention()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var charId = Guid.NewGuid();
        var stateId = Guid.NewGuid();
        var storagePath = $"novels/images/{imageId}.png";
        var images = new List<ImageExportInfo>
        {
            new(imageId, storagePath, $"char_{charId:N}_state_{stateId:N}")
        };

        var imageStream = new MemoryStream("image"u8.ToArray());
        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(storagePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(imageStream);

        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        // Act
        await _exporter.ExportAsync(archive, images, CancellationToken.None);
        archive.Dispose();

        // Assert
        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        var entry = readArchive.GetEntry($"game/images/char_{charId:N}_state_{stateId:N}.png");
        
        Assert.NotNull(entry);
        Assert.Equal($"game/images/char_{charId:N}_state_{stateId:N}.png", entry.FullName);
    }

    [Fact]
    public async Task ExportAsync_WithMultipleImages_CreatesAllEntries()
    {
        // Arrange
        var imageId1 = Guid.NewGuid();
        var imageId2 = Guid.NewGuid();
        var imageId3 = Guid.NewGuid();
        var storagePath1 = $"novels/images/{imageId1}.png";
        var storagePath2 = $"novels/images/{imageId2}.png";
        var storagePath3 = $"novels/images/{imageId3}.png";
        
        var images = new List<ImageExportInfo>
        {
            new(imageId1, storagePath1, "bg_room"),
            new(imageId2, storagePath2, "bg_park"),
            new(imageId3, storagePath3, "char_alice_happy")
        };

        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new MemoryStream("data"u8.ToArray()));

        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        // Act
        await _exporter.ExportAsync(archive, images, CancellationToken.None);
        archive.Dispose();

        // Assert
        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        
        Assert.Equal(3, readArchive.Entries.Count);
        Assert.NotNull(readArchive.GetEntry("game/images/bg_room.png"));
        Assert.NotNull(readArchive.GetEntry("game/images/bg_park.png"));
        Assert.NotNull(readArchive.GetEntry("game/images/char_alice_happy.png"));
    }

    [Fact]
    public async Task ExportAsync_WithMissingImage_ContinuesWithOtherImages()
    {
        // Arrange
        var imageId1 = Guid.NewGuid();
        var imageId2 = Guid.NewGuid();
        var storagePath1 = $"novels/images/{imageId1}.png";
        var storagePath2 = $"novels/images/{imageId2}.png";
        
        var images = new List<ImageExportInfo>
        {
            new(imageId1, storagePath1, "bg_room"),
            new(imageId2, storagePath2, "bg_park")
        };

        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(storagePath1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new FileNotFoundException());

        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(storagePath2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemoryStream("data"u8.ToArray()));

        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        // Act
        await _exporter.ExportAsync(archive, images, CancellationToken.None);
        archive.Dispose();

        // Assert
        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        
        Assert.Single(readArchive.Entries);
        Assert.NotNull(readArchive.GetEntry("game/images/bg_park.png"));
    }

    [Fact]
    public async Task ExportAsync_WithEmptyImageList_CreatesNoEntries()
    {
        // Arrange
        var images = new List<ImageExportInfo>();

        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        // Act
        await _exporter.ExportAsync(archive, images, CancellationToken.None);
        archive.Dispose();

        // Assert
        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        
        Assert.Empty(readArchive.Entries);
    }

    [Fact]
    public async Task ExportAsync_VerifiesStorageServiceCalled()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var storagePath = $"novels/images/{imageId}.png";
        var images = new List<ImageExportInfo>
        {
            new(imageId, storagePath, "test_image")
        };

        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(storagePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemoryStream("data"u8.ToArray()));

        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        // Act
        await _exporter.ExportAsync(archive, images, CancellationToken.None);

        // Assert
        _mockStorageService.Verify(
            x => x.DownloadFileStreamAsync(storagePath, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExportAsync_WithCancellationToken_PropagatesToken()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var storagePath = $"novels/images/{imageId}.png";
        var images = new List<ImageExportInfo>
        {
            new(imageId, storagePath, "test")
        };

        var cts = new CancellationTokenSource();
        
        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(storagePath, cts.Token))
            .ReturnsAsync(new MemoryStream("data"u8.ToArray()));

        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        // Act
        await _exporter.ExportAsync(archive, images, cts.Token);

        // Assert
        _mockStorageService.Verify(
            x => x.DownloadFileStreamAsync(storagePath, cts.Token),
            Times.Once);
    }

    [Fact]
    public async Task ExportAsync_WithBinaryData_PreservesData()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var storagePath = $"novels/images/{imageId}.png";
        var images = new List<ImageExportInfo>
        {
            new(imageId, storagePath, "binary_test")
        };

        var binaryData = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(storagePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemoryStream(binaryData));

        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        // Act
        await _exporter.ExportAsync(archive, images, CancellationToken.None);
        archive.Dispose();

        // Assert
        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        var entry = readArchive.GetEntry("game/images/binary_test.png");
        
        Assert.NotNull(entry);
        
        using var entryStream = entry.Open();
        using var ms = new MemoryStream();
        await entryStream.CopyToAsync(ms);
        
        Assert.Equal(binaryData, ms.ToArray());
    }

    [Fact]
    public async Task ExportAsync_WithLargeFile_HandlesCorrectly()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var storagePath = $"novels/images/{imageId}.png";
        var images = new List<ImageExportInfo>
        {
            new(imageId, storagePath, "large_file")
        };

        var largeData = new byte[1024 * 1024]; // 1MB
        new Random().NextBytes(largeData);
        
        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(storagePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemoryStream(largeData));

        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        // Act
        await _exporter.ExportAsync(archive, images, CancellationToken.None);
        archive.Dispose();

        // Assert
        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        var entry = readArchive.GetEntry("game/images/large_file.png");
        
        Assert.NotNull(entry);
        Assert.True(entry.Length > 0);
    }
}
