using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels;

namespace NoviVovi.Application.Characters.Features.Delete;

public record DeleteCharacterStateCommand(
    Guid NovelId,
    Guid CharacterId,
    Guid StateId
) : IRequest;

public class DeleteCharacterStateHandler(
    INovelRepository novelRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeleteCharacterStateCommand>
{
    public async Task Handle(DeleteCharacterStateCommand request, CancellationToken ct)
    {
        var novel = await novelRepository.GetByIdAsync(request.NovelId, ct)
                    ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");

        var character = novel.Characters.FirstOrDefault(c => c.Id == request.CharacterId)
                        ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");
        
        character.RemoveCharacterStateById(request.StateId);

        await unitOfWork.SaveChangesAsync(ct);
    }
}