using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Dialogue.Dtos;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class ShowBackgroundStepDtoMapper(
    TransformDtoMapper transformMapper
)
{
    public partial ShowBackgroundStepDto ToDto(ShowBackgroundStep subject);

    private TransformDto MapTransform(Transform source) => transformMapper.ToDto(source);

    public partial IEnumerable<ShowBackgroundStepDto> ToDtos(IEnumerable<ShowBackgroundStep> subjects);
}