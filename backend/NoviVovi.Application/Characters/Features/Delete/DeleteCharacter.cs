using MediatR;
using NoviVovi.Application.Characters.Abstactions;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Novels.Abstractions;

namespace NoviVovi.Application.Characters.Features.Delete;

public record DeleteCharacterCommand(
    Guid NovelId,
    Guid CharacterId
) : IRequest;

public class DeleteCharacterHandler(
    INovelRepository novelRepository,
    ICharacterRepository characterRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeleteCharacterCommand>
{
    public async Task Handle(DeleteCharacterCommand request, CancellationToken ct)
    {
        unitOfWork.BeginTransaction();
        
        try
        {
            var novel = await novelRepository.GetByIdAsync(request.NovelId, ct)
                        ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");
            
            var character = await characterRepository.GetByIdAsync(request.CharacterId, ct)
                            ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");
            
            if (novel.Characters.All(c => !c.Id.Equals(request.CharacterId)))
                throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден в новелле '{request.NovelId}'");
            
            novel.RemoveCharacterById(request.CharacterId);
            
            await characterRepository.DeleteAsync(character, ct);
            
            await novelRepository.AddOrUpdateAsync(novel, ct);

            await unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}