using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Novels.Abstractions;

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
        unitOfWork.BeginTransaction();
        
        try
        {
            var allCharacters = await novelRepository.GetAllCharactersAsync(request.NovelId, ct);
            var character = allCharacters.FirstOrDefault(c => c.Id == request.CharacterId)
                            ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");
            
            character.RemoveCharacterStateById(request.StateId);

            await unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}