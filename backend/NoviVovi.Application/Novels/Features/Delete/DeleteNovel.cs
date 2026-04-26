using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels.Abstractions;

namespace NoviVovi.Application.Novels.Features.Delete;

public record DeleteNovelCommand(
    Guid NovelId
) : IRequest;

public class DeleteNovelHandler(
    INovelRepository novelRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeleteNovelCommand>
{
    public async Task Handle(DeleteNovelCommand request, CancellationToken ct)
    {
        unitOfWork.BeginTransaction();
        
        try
        {
            var novel = await novelRepository.GetByIdAsync(request.NovelId, ct)
                        ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");
            
            await novelRepository.DeleteAsync(novel, ct);
            
            await unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}