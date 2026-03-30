using MediatR;
using NoviVovi.Application.Labels.Dtos;
using NoviVovi.Application.Labels.Features.Add;
using NoviVovi.Application.Labels.Mappers;
using NoviVovi.Application.Novels;

namespace NoviVovi.Application.Labels.Features.Patch;

public record PatchLabelCommand : IRequest<LabelDto>
{
    public required Guid NovelId { get; init; }
    public required Guid LabelId { get; init; }
    public string? Name { get; init; }
}

public class PatchLabelHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    LabelDtoMapper mapper
) : IRequestHandler<AddLabelCommand, LabelDto>
{
    public Task<LabelDto> Handle(AddLabelCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}