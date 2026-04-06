using NoviVovi.Api.Novels.Responses;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Models;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Novels.Mappers;

[Mapper]
public partial class EdgeResponseMapper
{
    [MapDerivedType(typeof(JumpEdgeDto), typeof(JumpEdgeResponse))]
    [MapDerivedType(typeof(ChoiceEdgeDto), typeof(ChoiceEdgeResponse))]
    public partial EdgeResponse ToResponse(EdgeDto source);
}