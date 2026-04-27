using NoviVovi.Application.Common.Abstractions;

namespace NoviVovi.Api.Tests.Infrastructure;

public class MockStorageService : IStorageService
{
    private readonly Dictionary<string, byte[]> _storage = [];

    public Task<string> GetPresignedUploadUrlAsync(string storagePath, CancellationToken ct = default)
    {
        // Return mock presigned URL
        return Task.FromResult($"https://mock-storage.test/upload/{storagePath}");
    }

    public string GetViewUrl(string storagePath)
    {
        // Return mock view URL
        return $"https://mock-storage.test/view/{storagePath}";
    }

    public Task DeleteFileAsync(string storagePath, CancellationToken ct = default)
    {
        _storage.Remove(storagePath);
        return Task.CompletedTask;
    }

    public Task<Stream> DownloadFileStreamAsync(string storagePath, CancellationToken ct = default)
    {
        // Return empty stream for tests
        if (_storage.TryGetValue(storagePath, out var data))
        {
            return Task.FromResult<Stream>(new MemoryStream(data));
        }
        
        return Task.FromResult<Stream>(new MemoryStream());
    }
}
