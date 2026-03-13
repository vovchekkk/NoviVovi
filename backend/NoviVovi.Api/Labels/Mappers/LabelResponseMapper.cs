using NoviVovi.Api.Labels.Responses;
using NoviVovi.Application.Labels.Contracts;
using NoviVovi.Domain.Labels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Labels.Mappers;

[Mapper]
public partial class LabelResponseMapper
{
    public partial LabelResponse ToSnapshot(LabelSnapshot novel);

    public partial IEnumerable<LabelResponse> ToSnapshots(IEnumerable<LabelSnapshot> novels);
}