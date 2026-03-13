using NoviVovi.Api.Scene.Responses;
using NoviVovi.Application.Scene.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Scene.Mappers;

[Mapper]
public partial class TransformResponseMapper
{
    public partial TransformResponse ToSnapshot(TransformSnapshot novel);
    
    public partial IEnumerable<TransformResponse> ToSnapshots(IEnumerable<TransformSnapshot> novels);
}