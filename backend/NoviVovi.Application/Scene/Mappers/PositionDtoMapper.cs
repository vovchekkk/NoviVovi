using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Scene.Mappers;

[Mapper]
public partial class PositionDtoMapper
{
    public partial PositionDto ToDto(Position source);
    
    public partial IEnumerable<PositionDto> ToDtos(IEnumerable<Position> sources);
}