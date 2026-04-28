using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Novels.Abstractions;

namespace NoviVovi.Application.Steps.Features.Delete;

public record DeleteStepCommand(
    Guid NovelId,
    Guid LabelId,
    Guid StepId
) : IRequest;

public class DeleteStepHandler(
    ILabelRepository labelRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeleteStepCommand>
{
    public async Task Handle(DeleteStepCommand request, CancellationToken ct)
    {
        unitOfWork.BeginTransaction();
        
        try
        {
            var label = await labelRepository.GetByIdAsync(request.LabelId, ct)
                        ?? throw new NotFoundException($"Метка '{request.LabelId}' не найдена");
            
            if (label.NovelId != request.NovelId)
                throw new ConflictException($"Метка '{request.LabelId}' не принадлежит новелле '{request.NovelId}'");

            // Check if step exists before trying to remove it
            var stepExists = label.Steps.Any(s => s.Id == request.StepId);
            if (!stepExists)
                throw new NotFoundException($"Шаг '{request.StepId}' не найден");

            label.RemoveStepById(request.StepId);

            await labelRepository.AddOrUpdateAsync(label, ct);
            
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}