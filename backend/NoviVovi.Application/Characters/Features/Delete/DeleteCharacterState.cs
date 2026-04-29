using MediatR;
using NoviVovi.Application.Characters.Abstactions;
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
    ICharacterRepository characterRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeleteCharacterStateCommand>
{
    public async Task Handle(DeleteCharacterStateCommand request, CancellationToken ct)
    {
        unitOfWork.BeginTransaction();

        try
        {
            var character = await characterRepository.GetByIdAsync(request.CharacterId, ct)
                            ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");

            if (character.CharacterStates.All(s => !s.Id.Equals(request.StateId)))
                throw new NotFoundException($"Состояние персонажа '{request.StateId}' не найдено");

            character.RemoveCharacterStateById(request.StateId);

            await characterRepository.AddOrUpdateAsync(character, ct);

            await unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}