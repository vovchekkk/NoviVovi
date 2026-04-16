using MediatR;
using NoviVovi.Application.Characters.Dtos;

namespace NoviVovi.Application.Characters.Features.Get;

public record GetCharactersQuery(
    Guid NovelId
) : IRequest<IEnumerable<CharacterDto>>;

public class GetCharactersHandler : IRequestHandler<GetCharactersQuery, IEnumerable<CharacterDto>>
{
    public async Task<IEnumerable<CharacterDto>> Handle(GetCharactersQuery request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}