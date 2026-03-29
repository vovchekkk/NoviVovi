using NoviVovi.Api.Steps.Responses;
using NoviVovi.Application.Steps.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.Mappers;

[Mapper]
public partial class ShowReplicaStepMapper
{
    public partial ShowReplicaStepResponse ToResponse(ShowReplicaStepDto subject);
    
    public partial IEnumerable<ShowReplicaStepResponse> ToResponses(IEnumerable<ShowReplicaStepDto> subjects);
}