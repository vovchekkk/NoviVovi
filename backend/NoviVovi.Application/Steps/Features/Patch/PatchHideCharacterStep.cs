using MediatR;
using NoviVovi.Application.Characters.Abstactions;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Features.Patch;

public record PatchHideCharacterStepCommand : PatchStepCommand
{
    public Guid? CharacterId { get; init; }
}

public class PatchHideCharacterStepHandler(
    ICharacterRepository characterRepository,
    ILabelRepository labelRepository,
    IUnitOfWork unitOfWork,
    StepDtoMapper mapper
) : BasePatchStepHandler(labelRepository), IRequestHandler<PatchHideCharacterStepCommand, StepDto>
{
    public async Task<StepDto> Handle(PatchHideCharacterStepCommand request, CancellationToken ct)
    {
        unitOfWork.BeginTransaction();
        
        try
        {
            var (label, step) = await GetStepContextOrThrow(request, ct);
            
            if (step is not HideCharacterStep hideCharacterStep)
                throw new BadRequestException($"Step {step.Id} is not {typeof(HideCharacterStep)}");
            
            Character? character = null;
            if (request.CharacterId.HasValue)
            {
                character = await characterRepository.GetByIdAsync(request.CharacterId.Value, ct)
                                ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");
            }
            
            hideCharacterStep.Update(character);
            
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