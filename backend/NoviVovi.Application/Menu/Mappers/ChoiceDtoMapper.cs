using NoviVovi.Application.Menu.Dtos;
using NoviVovi.Domain.Menu;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Menu.Mappers;

[Mapper]
public partial class ChoiceDtoMapper
{
    public partial ChoiceDto ToDto(Choice source);

    public partial IEnumerable<ChoiceDto> ToDtos(IEnumerable<Choice> sources);
}