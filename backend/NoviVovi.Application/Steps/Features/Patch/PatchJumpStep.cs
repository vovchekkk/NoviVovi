using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
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
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    IUnitOfWork unitOfWork,
    StepDtoMapper mapper
) : BasePatchStepHandler(novelRepository, labelRepository), IRequestHandler<PatchJumpStepCommand, StepDto>
{
    public async Task<StepDto> Handle(PatchJumpStepCommand request, CancellationToken ct)
    {
        var (_, _, step) = await GetStepContextOrThrow(request, ct);

        if (step is not JumpStep jumpStep)
            throw new BadRequestException($"Step {step.Id} is not {typeof(JumpStep)}");

        Label? targetLabel = null;
        if (request.TargetLabelId.HasValue)
        {
            targetLabel = await labelRepository.GetByIdAsync(request.TargetLabelId.Value, ct)
                              ?? throw new NotFoundException($"Метка '{request.TargetLabelId}' не найдена");
        }
        
        jumpStep.Update(targetLabel);

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.ToDto(step);
    }
}