using MediatR;

namespace NoviVovi.Application.Characters.Features.Delete;

public record DeleteCharacterStateCommand(
    Guid NovelId,
    Guid CharacterId,
    Guid StateId
) : IRequest;

public class DeleteCharacterStateHandler : IRequestHandler<DeleteCharacterCommand>
{
    public async Task Handle(DeleteCharacterCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}