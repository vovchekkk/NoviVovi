using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class HideCharacterStepDtoMapper
{
    public partial HideCharacterStepDto ToDto(HideCharacterStep subject);
    
    public partial IEnumerable<HideCharacterStepDto> ToDtos(IEnumerable<HideCharacterStep> subjects);
}