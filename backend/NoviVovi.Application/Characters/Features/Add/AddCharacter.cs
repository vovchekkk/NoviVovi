using MediatR;
using NoviVovi.Application.Characters.Dtos;

namespace NoviVovi.Application.Characters.Features.Add;

public record AddCharacterCommand : IRequest<CharacterDto>
{
    public required Guid NovelId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}

public class AddCharacterHandler : IRequestHandler<AddCharacterCommand, CharacterDto>
{
    public Task<CharacterDto> Handle(AddCharacterCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}