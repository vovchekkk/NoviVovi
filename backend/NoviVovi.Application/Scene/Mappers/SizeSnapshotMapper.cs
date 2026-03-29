using NoviVovi.Application.Scene.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Scene.Mappers;

[Mapper]
public partial class SizeSnapshotMapper
{
    public partial SizeSnapshot ToResponse(SizeSnapshot subject);
    
    public partial IEnumerable<SizeSnapshot> ToResponses(IEnumerable<SizeSnapshot> subjects);
}