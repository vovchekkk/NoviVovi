using NoviVovi.Api.Steps.Responses;
using NoviVovi.Application.Steps.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.Mappers;

[Mapper]
public partial class ShowMenuStepMapper
{
    public partial ShowMenuStepResponse ToResponse(ShowMenuStepSnapshot novel);
    
    public partial IEnumerable<ShowMenuStepResponse> ToResponses(IEnumerable<ShowMenuStepSnapshot> novels);
}