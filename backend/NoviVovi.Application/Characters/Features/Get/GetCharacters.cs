using MediatR;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Novels.Abstractions;

namespace NoviVovi.Application.Characters.Features.Get;

public record GetCharactersQuery(
    Guid NovelId
) : IRequest<IEnumerable<CharacterDto>>;

public class GetCharactersHandler(
    INovelRepository novelRepository,
    CharacterDtoMapper mapper
) : IRequestHandler<GetCharactersQuery, IEnumerable<CharacterDto>>
{
    public async Task<IEnumerable<CharacterDto>> Handle(GetCharactersQuery request, CancellationToken ct)
    {
        var allCharacters = await novelRepository.GetAllCharactersAsync(request.NovelId, ct);

        return mapper.ToDtos(allCharacters);
    }
}