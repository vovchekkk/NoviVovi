using MediatR;

namespace NoviVovi.Application.Characters.Features.Delete;

public record DeleteCharacterStateCommand(
    Guid NovelId,
    Guid CharacterId,
    Guid StateId
) : IRequest;

public class DeleteCharacterStateHandler : IRequestHandler<DeleteCharacterCommand>
{
    public Task Handle(DeleteCharacterCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}