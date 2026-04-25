using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NoviVovi.Api.Novels.Controllers;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels.Features.Export;

namespace NoviVovi.Api.Tests.Novels.Controllers;

/// <summary>
/// Tests for the Export endpoint in NovelsController.
/// </summary>
public class NovelsControllerExportTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly NovelsController _controller;

    public NovelsControllerExportTests()
    {
        _mockMediator = new Mock<IMediator>();
        
        // Controller requires multiple dependencies, but we only need mediator for export
        _controller = new NovelsController(
            _mockMediator.Object,
            null!, // commandMapper - not used in export
            null!, // novelMapper - not used in export
            null!  // novelGraphMapper - not used in export
        );
    }

    [Fact]
    public async Task ExportToRenPy_ValidNovelId_ReturnsZipFile()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var fakeZipBytes = new byte[] { 0x50, 0x4B, 0x03, 0x04 }; // ZIP header

        _mockMediator
            .Setup(m => m.Send(
                It.Is<ExportNovelToRenPyCommand>(c => c.NovelId == novelId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeZipBytes);

        // Act
        var result = await _controller.ExportToRenPy(novelId);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/zip", fileResult.ContentType);
        Assert.Equal("project.zip", fileResult.FileDownloadName);
        Assert.Equal(fakeZipBytes, fileResult.FileContents);
    }

    [Fact]
    public async Task ExportToRenPy_NonExistentNovel_ThrowsNotFoundException()
    {
        // Arrange
        var novelId = Guid.NewGuid();

        _mockMediator
            .Setup(m => m.Send(
                It.Is<ExportNovelToRenPyCommand>(c => c.NovelId == novelId),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Novel '{novelId}' not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _controller.ExportToRenPy(novelId)
        );
    }

    [Fact]
    public async Task ExportToRenPy_ValidNovel_CallsMediatorWithCorrectCommand()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var fakeZipBytes = new byte[] { 0x50, 0x4B };

        _mockMediator
            .Setup(m => m.Send(
                It.IsAny<ExportNovelToRenPyCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeZipBytes);

        // Act
        await _controller.ExportToRenPy(novelId);

        // Assert
        _mockMediator.Verify(
            m => m.Send(
                It.Is<ExportNovelToRenPyCommand>(c => c.NovelId == novelId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExportToRenPy_EmptyZip_ReturnsEmptyFile()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var emptyZipBytes = Array.Empty<byte>();

        _mockMediator
            .Setup(m => m.Send(
                It.IsAny<ExportNovelToRenPyCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyZipBytes);

        // Act
        var result = await _controller.ExportToRenPy(novelId);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Empty(fileResult.FileContents);
    }

    [Fact]
    public async Task ExportToRenPy_LargeZip_ReturnsCorrectSize()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var largeZipBytes = new byte[1024 * 1024]; // 1MB

        _mockMediator
            .Setup(m => m.Send(
                It.IsAny<ExportNovelToRenPyCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(largeZipBytes);

        // Act
        var result = await _controller.ExportToRenPy(novelId);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal(1024 * 1024, fileResult.FileContents.Length);
    }
}
