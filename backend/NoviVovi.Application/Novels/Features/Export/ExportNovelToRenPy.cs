using MediatR;
using NoviVovi.Application.Novels.Abstractions;

namespace NoviVovi.Application.Novels.Features.Export;

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