namespace NoviVovi.Application.Common.Abstractions;

public interface IStorageService
{
    public Task<string> GetPresignedUploadUrlAsync(string storagePath, CancellationToken ct);
    public string GetViewUrl(string storagePath);
    public Task DeleteFileAsync(string storagePath, CancellationToken ct);
    public Task<byte[]> DownloadFileAsync(string storagePath, CancellationToken ct);
}