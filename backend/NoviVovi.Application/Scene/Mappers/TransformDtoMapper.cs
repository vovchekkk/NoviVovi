using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Scene.Mappers;

[Mapper]
public partial class TransformDtoMapper
{
    public partial TransformDto ToDto(Transform subject);
    
    public partial IEnumerable<TransformDto> ToDtos(IEnumerable<Transform> subjects);
}