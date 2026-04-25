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
        var images = new List<ImageExportInfo>
        {
            new(imageId, $"bg_{imageId:N}")
        };

        var imageData = "fake image data"u8.ToArray();
        var imageStream = new MemoryStream(imageData);

        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(imageId.ToString(), It.IsAny<CancellationToken>()))
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
        using var reader = new MemoryStream();
        await entryStream.CopyToAsync(reader);
        Assert.Equal(imageData, reader.ToArray());
    }

    [Fact]
    public async Task ExportAsync_WithBackgroundImage_UsesCorrectNamingConvention()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var images = new List<ImageExportInfo>
        {
            new(imageId, $"bg_{imageId:N}")
        };

        var imageStream = new MemoryStream("image"u8.ToArray());
        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(imageId.ToString(), It.IsAny<CancellationToken>()))
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
        Assert.Equal($"game/images/bg_{imageId:N}.png", entry.FullName);
    }

    [Fact]
    public async Task ExportAsync_WithCharacterImage_UsesCorrectNamingConvention()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var charId = Guid.NewGuid();
        var stateId = Guid.NewGuid();
        var images = new List<ImageExportInfo>
        {
            new(imageId, $"char_{charId:N} state_{stateId:N}")
        };

        var imageStream = new MemoryStream("image"u8.ToArray());
        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(imageId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(imageStream);

        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        // Act
        await _exporter.ExportAsync(archive, images, CancellationToken.None);
        archive.Dispose();

        // Assert
        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        var entry = readArchive.GetEntry($"game/images/char_{charId:N} state_{stateId:N}.png");
        
        Assert.NotNull(entry);
        Assert.Equal($"game/images/char_{charId:N} state_{stateId:N}.png", entry.FullName);
    }

    [Fact]
    public async Task ExportAsync_WithMultipleImages_CreatesAllEntries()
    {
        // Arrange
        var imageId1 = Guid.NewGuid();
        var imageId2 = Guid.NewGuid();
        var imageId3 = Guid.NewGuid();
        
        var images = new List<ImageExportInfo>
        {
            new(imageId1, $"bg_{imageId1:N}"),
            new(imageId2, $"bg_{imageId2:N}"),
            new(imageId3, $"bg_{imageId3:N}")
        };

        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string id, CancellationToken ct) => new MemoryStream(Encoding.UTF8.GetBytes($"data_{id}")));

        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        // Act
        await _exporter.ExportAsync(archive, images, CancellationToken.None);
        archive.Dispose();

        // Assert
        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        Assert.Equal(3, readArchive.Entries.Count);
        Assert.NotNull(readArchive.GetEntry($"game/images/bg_{imageId1:N}.png"));
        Assert.NotNull(readArchive.GetEntry($"game/images/bg_{imageId2:N}.png"));
        Assert.NotNull(readArchive.GetEntry($"game/images/bg_{imageId3:N}.png"));
    }

    [Fact]
    public async Task ExportAsync_WithMissingImage_ContinuesExportingOtherImages()
    {
        // Arrange
        var imageId1 = Guid.NewGuid();
        var imageId2 = Guid.NewGuid();
        var imageId3 = Guid.NewGuid();
        
        var images = new List<ImageExportInfo>
        {
            new(imageId1, $"bg_{imageId1:N}"),
            new(imageId2, $"bg_{imageId2:N}"),
            new(imageId3, $"bg_{imageId3:N}")
        };

        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(imageId1.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemoryStream("data1"u8.ToArray()));

        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(imageId2.ToString(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new FileNotFoundException("Image not found"));

        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(imageId3.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemoryStream("data3"u8.ToArray()));

        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        // Act
        await _exporter.ExportAsync(archive, images, CancellationToken.None);
        archive.Dispose();

        // Assert
        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        Assert.Equal(2, readArchive.Entries.Count);
        Assert.NotNull(readArchive.GetEntry($"game/images/bg_{imageId1:N}.png"));
        Assert.Null(readArchive.GetEntry($"game/images/bg_{imageId2:N}.png"));
        Assert.NotNull(readArchive.GetEntry($"game/images/bg_{imageId3:N}.png"));
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
    public async Task ExportAsync_CallsStorageServiceWithCorrectImageId()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var images = new List<ImageExportInfo>
        {
            new(imageId, $"bg_{imageId:N}")
        };

        var imageStream = new MemoryStream("image"u8.ToArray());
        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(imageId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(imageStream);

        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        // Act
        await _exporter.ExportAsync(archive, images, CancellationToken.None);

        // Assert
        _mockStorageService.Verify(
            x => x.DownloadFileStreamAsync(imageId.ToString(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExportAsync_WithCancellationToken_PropagatesToken()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var images = new List<ImageExportInfo>
        {
            new(imageId, $"bg_{imageId:N}")
        };

        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        var imageStream = new MemoryStream("image"u8.ToArray());
        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(imageId.ToString(), ct))
            .ReturnsAsync(imageStream);

        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        // Act
        await _exporter.ExportAsync(archive, images, ct);

        // Assert
        _mockStorageService.Verify(
            x => x.DownloadFileStreamAsync(imageId.ToString(), ct),
            Times.Once);
    }

    [Fact]
    public async Task ExportAsync_PreservesImageBinaryData()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var images = new List<ImageExportInfo>
        {
            new(imageId, $"bg_{imageId:N}")
        };

        // Simulate real PNG header bytes
        var imageData = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00 };
        var imageStream = new MemoryStream(imageData);

        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(imageId.ToString(), It.IsAny<CancellationToken>()))
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
        using var reader = new MemoryStream();
        await entryStream.CopyToAsync(reader);
        Assert.Equal(imageData, reader.ToArray());
    }

    [Fact]
    public async Task ExportAsync_WithLargeImage_HandlesBufferSizeCorrectly()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var images = new List<ImageExportInfo>
        {
            new(imageId, $"bg_{imageId:N}")
        };

        // Create 1MB of data
        var imageData = new byte[1024 * 1024];
        new Random(42).NextBytes(imageData);
        var imageStream = new MemoryStream(imageData);

        _mockStorageService
            .Setup(x => x.DownloadFileStreamAsync(imageId.ToString(), It.IsAny<CancellationToken>()))
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
        using var reader = new MemoryStream();
        await entryStream.CopyToAsync(reader);
        Assert.Equal(imageData.Length, reader.ToArray().Length);
    }
}
