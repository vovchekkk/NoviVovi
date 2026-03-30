using NoviVovi.Api.Steps.Responses;
using NoviVovi.Application.Steps.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.Mappers;

[Mapper]
public partial class JumpStepResponseMapper
{
    public partial JumpStepResponse ToResponse(JumpStepDto subject);
    
    public partial IEnumerable<JumpStepResponse> ToResponses(IEnumerable<JumpStepDto> subjects);
}