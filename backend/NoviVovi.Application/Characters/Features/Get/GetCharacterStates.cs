using MediatR;
using NoviVovi.Application.Characters.Dtos;

namespace NoviVovi.Application.Characters.Features.Get;

public record GetCharacterStatesQuery(
    Guid NovelId,
    Guid CharacterId
) : IRequest<IEnumerable<CharacterStateDto>>;

public class GetCharacterStatesHandler : IRequestHandler<GetCharacterStatesQuery, IEnumerable<CharacterStateDto>>
{
    public async Task<IEnumerable<CharacterStateDto>> Handle(GetCharacterStatesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}