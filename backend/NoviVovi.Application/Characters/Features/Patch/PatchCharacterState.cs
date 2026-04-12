using MediatR;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Scene.Dtos;

namespace NoviVovi.Application.Characters.Features.Patch;

public record PatchCharacterStateCommand : IRequest<CharacterStateDto>
{
    public required Guid NovelId { get; init; }
    public required Guid CharacterId { get; init; }
    public required Guid StateId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public Guid? ImageId { get; init; }
    public TransformDto? Transform { get; init; }
}

public class PatchCharacterStateHandler : IRequestHandler<PatchCharacterStateCommand, CharacterStateDto>
{
    public async Task<CharacterStateDto> Handle(PatchCharacterStateCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}