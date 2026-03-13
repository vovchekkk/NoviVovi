using NoviVovi.Application.Scene.Contracts;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Scene.Mappers;

[Mapper]
public partial class PositionMapper
{
    public partial PositionSnapshot ToSnapshot(Position novel);
    
    public partial IEnumerable<PositionSnapshot> ToSnapshots(IEnumerable<Position> novels);
}