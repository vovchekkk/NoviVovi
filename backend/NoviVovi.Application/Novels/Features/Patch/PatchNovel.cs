using MediatR;
using NoviVovi.Application.Novels.Dtos;

namespace NoviVovi.Application.Novels.Features.Patch;

public record PatchNovelCommand : IRequest<NovelDto>
{
    public required Guid NovelId { get; init; }
    public string? Title { get; init; }
    public Guid? StartLabelId { get; init; }
}

public class PatchNovelHandler : IRequestHandler<PatchNovelCommand, NovelDto>
{
    public async Task<NovelDto> Handle(PatchNovelCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}