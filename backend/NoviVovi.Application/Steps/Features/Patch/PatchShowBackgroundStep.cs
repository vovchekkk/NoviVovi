using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Images;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Features.Patch;

public record PatchShowBackgroundStepCommand : PatchStepCommand
{
    public Guid? ImageId { get; init; }
    public TransformDto? Transform { get; init; }
}

public class PatchShowBackgroundStepHandler(
    ILabelRepository labelRepository,
    IImageRepository imageRepository,
    TransformDtoMapper transformMapper,
    IUnitOfWork unitOfWork,
    StepDtoMapper mapper
) : BasePatchStepHandler(labelRepository), IRequestHandler<PatchShowBackgroundStepCommand, StepDto>
{
    public async Task<StepDto> Handle(PatchShowBackgroundStepCommand request, CancellationToken ct)
    {
        var (_, step) = await GetStepContextOrThrow(request, ct);

        if (step is not ShowBackgroundStep showBackgroundStep)
            throw new BadRequestException($"Step {step.Id} is not {typeof(ShowBackgroundStep)}");

        Image? image = null;
        if (request.ImageId.HasValue)
        {
            image = await imageRepository.GetByIdAsync(request.ImageId.Value, ct)
                    ?? throw new NotFoundException($"Изображение '{request.ImageId}' не найдено");
        }

        var transformPatch = request.Transform != null 
            ? transformMapper.ToDomainPatch(request.Transform) 
            : null;

        showBackgroundStep.Update(image, transformPatch);

        await unitOfWork.SaveChangesAsync(ct);
        return mapper.ToDto(step);
    }
}