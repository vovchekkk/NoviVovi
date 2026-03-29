using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Labels.Responses;

namespace NoviVovi.Api.Novels.Responses;

public record NovelResponse(
    Guid Id,
    string Title,
    LabelResponse StartLabel,
    List<LabelResponse> Labels,
    List<CharacterResponse> Characters
);