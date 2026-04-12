using MediatR;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Scene.Dtos;

namespace NoviVovi.Application.Characters.Features.Add;

public record AddCharacterStateCommand : IRequest<CharacterStateDto>
{
    public required Guid NovelId { get; init; }
    public required Guid CharacterId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required Guid ImageId { get; init; }
    public required TransformDto? Transform { get; init; }
}

public class AddCharacterStateHandler : IRequestHandler<AddCharacterStateCommand, CharacterStateDto>
{
    public async Task<CharacterStateDto> Handle(AddCharacterStateCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}