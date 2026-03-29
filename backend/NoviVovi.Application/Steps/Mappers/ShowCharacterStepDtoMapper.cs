using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class ShowCharacterStepDtoMapper
{
    public partial ShowCharacterStepDto ToDto(ShowCharacterStep subject);
    
    public partial IEnumerable<ShowCharacterStepDto> ToDtos(IEnumerable<ShowCharacterStep> subjects);
}