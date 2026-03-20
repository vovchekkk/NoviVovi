using NoviVovi.Application.Characters.Contracts;
using NoviVovi.Application.Labels.Contracts;

namespace NoviVovi.Application.Novels.Contracts;

public record NovelSnapshot(
    string Title,
    LabelSnapshot StartLabel,
    List<LabelSnapshot> Labels,
    List<CharacterSnapshot> Characters
);