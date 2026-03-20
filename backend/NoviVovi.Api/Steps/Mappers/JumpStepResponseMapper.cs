using NoviVovi.Api.Steps.Responses;
using NoviVovi.Application.Steps.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.Mappers;

[Mapper]
public partial class JumpStepMapper
{
    public partial JumpStepResponse ToResponse(JumpStepSnapshot subject);
    
    public partial IEnumerable<JumpStepResponse> ToResponses(IEnumerable<JumpStepSnapshot> subjects);
}