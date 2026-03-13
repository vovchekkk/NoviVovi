using NoviVovi.Api.Menu.Responses;
using NoviVovi.Application.Menu.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Menu.Mappers;

[Mapper]
public partial class ChoiceResponseMapper
{
    public partial ChoiceResponse ToSnapshot(ChoiceSnapshot novel);

    public partial IEnumerable<ChoiceResponse> ToSnapshots(IEnumerable<ChoiceSnapshot> novels);
}