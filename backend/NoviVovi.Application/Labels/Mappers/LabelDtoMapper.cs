using NoviVovi.Application.Labels.Dtos;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Labels.Mappers;

[Mapper]
public partial class LabelDtoMapper(
    StepDtoMapper stepMapper
)
{
    public partial LabelDto ToDto(Label source);

    private StepDto MapStep(Step source) => stepMapper.ToDto(source);

    public partial IEnumerable<LabelDto> ToDtos(IEnumerable<Label> sources);
}