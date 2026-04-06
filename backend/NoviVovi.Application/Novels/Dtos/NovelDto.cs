using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Labels.Dtos;

namespace NoviVovi.Application.Novels.Dtos;

public record NovelDto(
    Guid Id,
    string Title,
    Guid StartLabelId,
    List<Guid> LabelIds,
    List<Guid> CharacterIds
);