using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Novels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Novels.Mappers;

[Mapper]
public partial class NovelDtoMapper
{
    [MapProperty("StartLabel.Id", nameof(NovelDto.StartLabelId))]
    [MapProperty(nameof(Novel.Labels), nameof(NovelDto.LabelIds))]
    [MapProperty(nameof(Novel.Characters), nameof(NovelDto.CharacterIds))]
    public partial NovelDto ToDto(Novel subject);
    
    private Guid MapLabelToId(Label label) => label.Id;
    private Guid MapCharacterToId(Character character) => character.Id;

    public partial IEnumerable<NovelDto> ToDtos(IEnumerable<Novel> subjects);
}