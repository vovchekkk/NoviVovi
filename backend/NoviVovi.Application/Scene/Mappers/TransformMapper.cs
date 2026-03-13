using NoviVovi.Application.Scene.Contracts;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Scene.Mappers;

[Mapper]
public partial class TransformMapper
{
    public partial TransformSnapshot ToSnapshot(Transform novel);
    
    public partial IEnumerable<TransformSnapshot> ToSnapshots(IEnumerable<Transform> novels);
}