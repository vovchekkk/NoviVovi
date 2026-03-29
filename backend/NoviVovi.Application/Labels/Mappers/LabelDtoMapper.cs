using NoviVovi.Application.Labels.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Labels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Labels.Mappers;

[Mapper]
public partial class LabelDtoMapper
{
    public partial LabelDto ToDto(Label subject);

    public partial IEnumerable<LabelDto> ToDtos(IEnumerable<Label> subjects);
}