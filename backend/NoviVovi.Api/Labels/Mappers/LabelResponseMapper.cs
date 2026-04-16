using NoviVovi.Api.Labels.Responses;
using NoviVovi.Api.Steps.Mappers;
using NoviVovi.Api.Steps.Responses;
using NoviVovi.Application.Labels.Dtos;
using NoviVovi.Application.Steps.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Labels.Mappers;

[Mapper]
public partial class LabelResponseMapper(
    StepResponseMapper stepMapper
)
{
    public partial LabelResponse ToResponse(LabelDto source);

    private StepResponse MapStep(StepDto source) => stepMapper.ToResponse(source);

    public partial IEnumerable<LabelResponse> ToResponses(IEnumerable<LabelDto> sources);
}