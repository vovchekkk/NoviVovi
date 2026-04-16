using MediatR;
using NoviVovi.Application.Characters.Dtos;

namespace NoviVovi.Application.Characters.Features.Get;

public record GetCharacterQuery(
    Guid NovelId,
    Guid CharacterId
) : IRequest<CharacterDto>;

public class GetCharacterHandler : IRequestHandler<GetCharacterQuery, CharacterDto>
{
    public async Task<CharacterDto> Handle(GetCharacterQuery request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}