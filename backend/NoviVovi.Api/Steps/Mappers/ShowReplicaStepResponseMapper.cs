using NoviVovi.Api.Steps.Responses;
using NoviVovi.Application.Steps.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.Mappers;

[Mapper]
public partial class ShowReplicaStepMapper
{
    public partial ShowReplicaStepResponse ToResponse(ShowReplicaStepSnapshot subject);
    
    public partial IEnumerable<ShowReplicaStepResponse> ToResponses(IEnumerable<ShowReplicaStepSnapshot> subjects);
}