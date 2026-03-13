using NoviVovi.Api.Scene.Responses;
using NoviVovi.Application.Scene.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Scene.Mappers;

[Mapper]
public partial class PositionResponseMapper
{
    public partial PositionResponse ToSnapshot(PositionSnapshot novel);
    
    public partial IEnumerable<PositionResponse> ToSnapshots(IEnumerable<PositionSnapshot> novels);
}