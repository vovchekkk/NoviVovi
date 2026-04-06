using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Labels.Responses;

namespace NoviVovi.Api.Novels.Responses;

public record NovelResponse(
    Guid Id,
    string Title,
    Guid StartLabelId,
    List<Guid> LabelIds,
    List<Guid> CharacterIds
);