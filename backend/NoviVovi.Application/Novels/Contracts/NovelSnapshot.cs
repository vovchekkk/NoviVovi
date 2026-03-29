using NoviVovi.Application.Characters.Contracts;
using NoviVovi.Application.Labels.Contracts;

namespace NoviVovi.Application.Novels.Contracts;

public record NovelSnapshot(
    Guid Id,
    string Title,
    LabelSnapshot StartLabel,
    List<LabelSnapshot> Labels,
    List<CharacterSnapshot> Characters
);