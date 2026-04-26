using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Features.Patch;

public record PatchJumpStepCommand : PatchStepCommand
{
    public Guid? TargetLabelId { get; init; }
}

public class PatchJumpStepHandler(
    ILabelRepository labelRepository,
    IUnitOfWork unitOfWork,
    StepDtoMapper mapper
) : BasePatchStepHandler(labelRepository), IRequestHandler<PatchJumpStepCommand, StepDto>
{
    private readonly ILabelRepository _labelRepository = labelRepository;

    public async Task<StepDto> Handle(PatchJumpStepCommand request, CancellationToken ct)
    {
        unitOfWork.BeginTransaction();
        
        try
        {
            var (label, step) = await GetStepContextOrThrow(request, ct);

            if (step is not JumpStep jumpStep)
                throw new BadRequestException($"Step {step.Id} is not {typeof(JumpStep)}");

            Label? targetLabel = null;
            if (request.TargetLabelId.HasValue)
            {
                targetLabel = await _labelRepository.GetByIdAsync(request.TargetLabelId.Value, ct)
                                  ?? throw new NotFoundException($"Метка '{request.TargetLabelId}' не найдена");
            }
            
            jumpStep.Update(targetLabel);
            
            await labelRepository.AddOrUpdateAsync(label, ct);

            await unitOfWork.CommitAsync(ct);

            return mapper.ToDto(step);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}