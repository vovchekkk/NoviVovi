using MediatR;
using NoviVovi.Application.Novels.Dtos;

namespace NoviVovi.Application.Novels.Features.Patch;

public record PatchNovelCommand : IRequest<NovelDto>
{
    
}

public class PatchNovelHandler : IRequestHandler<PatchNovelCommand, NovelDto>
{
    public async Task<NovelDto> Handle(PatchNovelCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}