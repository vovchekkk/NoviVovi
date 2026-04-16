using MediatR;
using NoviVovi.Application.Characters.Dtos;

namespace NoviVovi.Application.Characters.Features.Get;

public record GetCharacterStateQuery(
    Guid NovelId,
    Guid CharacterId,
    Guid StateId
) : IRequest<CharacterStateDto>;

public class GetCharacterStateHandler : IRequestHandler<GetCharacterStateQuery, CharacterStateDto>
{
    public async Task<CharacterStateDto> Handle(GetCharacterStateQuery request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}