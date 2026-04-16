using NoviVovi.Api.Dialogue.Responses;
using NoviVovi.Application.Dialogue.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Dialogue.Mappers;

[Mapper]
public partial class ReplicaResponseMapper
{
    public partial ReplicaResponse ToResponse(ReplicaDto source);

    public partial IEnumerable<ReplicaResponse> ToResponses(IEnumerable<ReplicaDto> sources);
}