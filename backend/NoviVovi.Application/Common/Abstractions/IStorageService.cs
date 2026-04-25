namespace NoviVovi.Application.Common.Abstractions;

public interface IStorageService
{
    Task<string> GetPresignedUploadUrlAsync(string storagePath, CancellationToken ct);
    string GetViewUrl(string storagePath);
    Task DeleteFileAsync(string storagePath, CancellationToken ct);
    Task<Stream> DownloadFileStreamAsync(string storagePath, CancellationToken ct);
}