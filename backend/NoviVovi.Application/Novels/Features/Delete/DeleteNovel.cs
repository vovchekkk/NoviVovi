using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;

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
        var novel = await novelRepository.GetByIdAsync(request.NovelId, ct)
                    ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");
        
        await novelRepository.DeleteAsync(novel, ct);
        
        await unitOfWork.SaveChangesAsync(ct);
    }
}