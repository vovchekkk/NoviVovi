using System.IO.Compression;
using MediatR;
using NoviVovi.Application.Export.Abstractions;

namespace NoviVovi.Application.Export.Features.Export;

public record ExportNovelToRenPyCommand(
    Guid NovelId
) : IRequest<byte[]>;

public class ExportNovelToRenPyHandler(
    IExporter exporter
) : IRequestHandler<ExportNovelToRenPyCommand, byte[]>
{
    public async Task<byte[]> Handle(ExportNovelToRenPyCommand request, CancellationToken ct)
    {
        return await exporter.ExportToRenPyAsync(request.NovelId, ct);
    }
}