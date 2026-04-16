namespace NoviVovi.Application.Common;

public interface IStorageService
{
    public Task<string> GetPresignedUploadUrlAsync(string storagePath, CancellationToken ct);
    public string GetViewUrl(string storagePath);
    public Task DeleteFileAsync(string storagePath, CancellationToken ct);
}