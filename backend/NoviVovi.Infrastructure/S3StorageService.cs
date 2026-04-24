using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;

namespace NoviVovi.Infrastructure;

public class S3StorageService : IStorageService
{
    public async Task<string> GetPresignedUploadUrlAsync(string storagePath, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public string GetViewUrl(string storagePath) // тут написать что-то осмысленное
    {
        if (string.IsNullOrEmpty(storagePath)) return string.Empty;
        // Здесь логика генерации ссылки (через AWS SDK или просто склейка строк)
        return $"https://cdn.novivovi.com/{storagePath}";
    }

    public async Task DeleteFileAsync(string storagePath, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<byte[]> DownloadFileAsync(string storagePath, CancellationToken ct)
    {
        // TODO: Implement actual S3 download logic
        throw new NotImplementedException("S3 file download not yet implemented");
    }
}