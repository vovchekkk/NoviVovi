using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Export.Abstractions;

public interface IExporter
{
    Task<byte[]> ExportToRenPyAsync(Guid novelId, CancellationToken ct);
}