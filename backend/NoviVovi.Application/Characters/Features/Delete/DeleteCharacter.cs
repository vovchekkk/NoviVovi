using MediatR;
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
            
            novel.RemoveCharacterById(request.CharacterId);

            await unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}