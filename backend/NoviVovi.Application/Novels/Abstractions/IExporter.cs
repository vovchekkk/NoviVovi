namespace NoviVovi.Application.Novels.Abstractions;

public interface IExporter
{
    Task<byte[]> ExportToRenPyAsync(Guid novelId, CancellationToken ct);
}