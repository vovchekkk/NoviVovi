using MediatR;

namespace NoviVovi.Application.Characters.Features.Delete;

public record DeleteCharacterCommand(
    Guid NovelId,
    Guid CharacterId
) : IRequest;

public class DeleteCharacterHandler : IRequestHandler<DeleteCharacterCommand>
{
    public async Task Handle(DeleteCharacterCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}