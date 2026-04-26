using NoviVovi.Application.Common.Abstractions;

namespace NoviVovi.Infrastructure;

/// <summary>
/// Local storage service for development/testing.
/// Returns mock URLs instead of actual S3 operations.
/// </summary>
public class LocalStorageService : IStorageService
{
    private readonly string _baseUrl;

    public LocalStorageService(string baseUrl = "http://localhost:5136/storage")
    {
        _baseUrl = baseUrl;
    }

    public Task<string> GetPresignedUploadUrlAsync(string storagePath, CancellationToken ct)
    {
        // Return mock presigned URL for local testing
        var url = $"{_baseUrl}/upload/{storagePath}";
        return Task.FromResult(url);
    }

    public string GetViewUrl(string storagePath)
    {
        if (string.IsNullOrEmpty(storagePath)) return string.Empty;
        return $"{_baseUrl}/view/{storagePath}";
    }

    public Task DeleteFileAsync(string storagePath, CancellationToken ct)
    {
        // Mock delete - do nothing in local mode
        return Task.CompletedTask;
    }

    public Task<Stream> DownloadFileStreamAsync(string storagePath, CancellationToken ct)
    {
        // Return empty stream for local testing
        var stream = new MemoryStream();
        return Task.FromResult<Stream>(stream);
    }
}
