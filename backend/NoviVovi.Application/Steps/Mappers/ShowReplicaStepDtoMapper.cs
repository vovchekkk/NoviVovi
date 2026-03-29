using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class ShowReplicaStepDtoMapper
{
    public partial ShowReplicaStepDto ToDto(ShowReplicaStep subject);
    
    public partial IEnumerable<ShowReplicaStepDto> ToDtos(IEnumerable<ShowReplicaStep> subjects);
}