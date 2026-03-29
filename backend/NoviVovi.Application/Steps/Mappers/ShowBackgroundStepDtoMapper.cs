using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class ShowBackgroundStepDtoMapper
{
    public partial ShowBackgroundStepDto ToDto(ShowBackgroundStep subject);
    
    public partial IEnumerable<ShowBackgroundStepDto> ToDtos(IEnumerable<ShowBackgroundStep> subjects);
}