using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class JumpStepDtoMapper
{
    public partial JumpStepDto ToDto(JumpStep subject);
    
    public partial IEnumerable<JumpStepDto> ToDtos(IEnumerable<JumpStep> subjects);
}