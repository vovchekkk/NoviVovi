using MediatR;
using NoviVovi.Application.Characters.Dtos;

namespace NoviVovi.Application.Characters.Features.Get;

public record GetCharactersQuery(
    Guid NovelId
) : IRequest<IEnumerable<CharacterDto>>;

public class GetCharactersHandler : IRequestHandler<GetCharactersQuery, IEnumerable<CharacterDto>>
{
    public Task<IEnumerable<CharacterDto>> Handle(GetCharactersQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}