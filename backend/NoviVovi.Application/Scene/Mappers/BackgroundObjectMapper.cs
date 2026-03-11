using NoviVovi.Application.Scene.Contracts;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Scene.Mappers;

[Mapper]
public partial class BackgroundObjectMapper
{
    public partial BackgroundObjectSnapshot ToSnapshot(BackgroundObject novel);
    
    public partial IEnumerable<BackgroundObjectSnapshot> ToSnapshots(IEnumerable<BackgroundObject> novels);
}