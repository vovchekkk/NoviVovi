using MediatR;
using NoviVovi.Application.Characters.Dtos;

namespace NoviVovi.Application.Characters.Features.Patch;

public record PatchCharacterCommand : IRequest<CharacterDto>
{
    public required Guid NovelId { get; init; }
    public required Guid CharacterId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
}

public class PatchCharacterHandler : IRequestHandler<PatchCharacterCommand, CharacterDto>
{
    public async Task<CharacterDto> Handle(PatchCharacterCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}