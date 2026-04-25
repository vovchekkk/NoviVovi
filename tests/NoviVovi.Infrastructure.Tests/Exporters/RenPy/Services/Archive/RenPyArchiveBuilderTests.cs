using System.IO.Compression;
using System.Text;
using Moq;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Archive;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Resources;

namespace NoviVovi.Infrastructure.Tests.Exporters.RenPy.Services.Archive;

public class RenPyArchiveBuilderTests
{
    private readonly Mock<IEmbeddedResourceLoader> _mockResourceLoader;
    private readonly RenPyArchiveBuilder _builder;

    public RenPyArchiveBuilderTests()
    {
        _mockResourceLoader = new Mock<IEmbeddedResourceLoader>();
        _builder = new RenPyArchiveBuilder(_mockResourceLoader.Object);
    }

    [Fact]
    public void AddTextFile_WithUtf8Content_CreatesEntryWithCorrectContent()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);
        const string path = "game/script.rpy";
        const string content = "label start:\n    \"Hello World\"";

        // Act
        _builder.AddTextFile(archive, path, content);
        archive.Dispose();

        // Assert
        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        var entry = readArchive.GetEntry(path);
        
        Assert.NotNull(entry);
        using var reader = new StreamReader(entry.Open(), Encoding.UTF8);
        var actualContent = reader.ReadToEnd();
        Assert.Equal(content, actualContent);
    }

    [Fact]
    public void AddTextFile_WithCyrillicCharacters_PreservesEncoding()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);
        const string path = "game/script.rpy";
        const string content = "label start:\n    \"Привет, мир! 你好世界\"";

        // Act
        _builder.AddTextFile(archive, path, content);
        archive.Dispose();

        // Assert
        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        var entry = readArchive.GetEntry(path);
        
        Assert.NotNull(entry);
        using var reader = new StreamReader(entry.Open(), Encoding.UTF8);
        var actualContent = reader.ReadToEnd();
        Assert.Equal(content, actualContent);
    }

    [Fact]
    public void AddTextFile_WithEmojis_PreservesSpecialCharacters()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);
        const string path = "game/script.rpy";
        const string content = "label start:\n    \"Hello 👋 World 🌍 Test 🎮\"";

        // Act
        _builder.AddTextFile(archive, path, content);
        archive.Dispose();

        // Assert
        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        var entry = readArchive.GetEntry(path);
        
        Assert.NotNull(entry);
        using var reader = new StreamReader(entry.Open(), Encoding.UTF8);
        var actualContent = reader.ReadToEnd();
        Assert.Equal(content, actualContent);
    }

    [Fact]
    public void AddTextFile_WithMultipleFiles_CreatesAllEntries()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        // Act
        _builder.AddTextFile(archive, "game/script.rpy", "content1");
        _builder.AddTextFile(archive, "game/options.rpy", "content2");
        _builder.AddTextFile(archive, "game/screens.rpy", "content3");
        archive.Dispose();

        // Assert
        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        Assert.Equal(3, readArchive.Entries.Count);
        Assert.NotNull(readArchive.GetEntry("game/script.rpy"));
        Assert.NotNull(readArchive.GetEntry("game/options.rpy"));
        Assert.NotNull(readArchive.GetEntry("game/screens.rpy"));
    }

    [Fact]
    public async Task AddBaseProjectFilesAsync_WithValidBaseProject_CopiesAllEntries()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        // Create mock base project ZIP
        var baseProjectStream = new MemoryStream();
        using (var baseArchive = new ZipArchive(baseProjectStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            var entry1 = baseArchive.CreateEntry("game/gui.rpy");
            using (var writer1 = new StreamWriter(entry1.Open()))
            {
                writer1.Write("# GUI configuration");
            }

            var entry2 = baseArchive.CreateEntry("game/images/logo.png");
            using (var writer2 = new StreamWriter(entry2.Open()))
            {
                writer2.Write("fake image data");
            }
        }
        baseProjectStream.Position = 0;

        _mockResourceLoader
            .Setup(x => x.LoadStreamResourceAsync(
                "NoviVovi.Infrastructure.Exporters.RenPy.Resources.BaseProject.zip",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(baseProjectStream);

        // Act
        await _builder.AddBaseProjectFilesAsync(archive, CancellationToken.None);
        archive.Dispose();

        // Assert
        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        Assert.Equal(2, readArchive.Entries.Count);
        Assert.NotNull(readArchive.GetEntry("game/gui.rpy"));
        Assert.NotNull(readArchive.GetEntry("game/images/logo.png"));
    }

    [Fact]
    public async Task AddBaseProjectFilesAsync_WithMissingBaseProject_DoesNotThrow()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        _mockResourceLoader
            .Setup(x => x.LoadStreamResourceAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Stream?)null);

        // Act & Assert
        await _builder.AddBaseProjectFilesAsync(archive, CancellationToken.None);
        archive.Dispose();

        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        Assert.Empty(readArchive.Entries);
    }

    [Fact]
    public async Task AddBaseProjectFilesAsync_PreservesFileContent()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);

        const string expectedContent = "# RenPy GUI Configuration\ndefine gui.text_size = 22";
        var baseProjectStream = new MemoryStream();
        using (var baseArchive = new ZipArchive(baseProjectStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            var baseEntry = baseArchive.CreateEntry("game/gui.rpy");
            using var writer = new StreamWriter(baseEntry.Open(), Encoding.UTF8);
            writer.Write(expectedContent);
        }
        baseProjectStream.Position = 0;

        _mockResourceLoader
            .Setup(x => x.LoadStreamResourceAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(baseProjectStream);

        // Act
        await _builder.AddBaseProjectFilesAsync(archive, CancellationToken.None);
        archive.Dispose();

        // Assert
        memoryStream.Position = 0;
        using var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        var entry = readArchive.GetEntry("game/gui.rpy");
        Assert.NotNull(entry);
        
        using var reader = new StreamReader(entry.Open(), Encoding.UTF8);
        var actualContent = reader.ReadToEnd();
        Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public async Task AddBaseProjectFilesAsync_WithCancellationToken_PropagatesToken()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        _mockResourceLoader
            .Setup(x => x.LoadStreamResourceAsync(
                It.IsAny<string>(),
                ct))
            .ReturnsAsync((Stream?)null);

        // Act
        await _builder.AddBaseProjectFilesAsync(archive, ct);

        // Assert
        _mockResourceLoader.Verify(
            x => x.LoadStreamResourceAsync(It.IsAny<string>(), ct),
            Times.Once);
    }
}
