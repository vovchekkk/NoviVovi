using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class ShowMenuStepDtoMapper
{
    public partial ShowMenuStepDto ToDto(ShowMenuStep subject);
    
    public partial IEnumerable<ShowMenuStepDto> ToDtos(IEnumerable<ShowMenuStep> subjects);
}