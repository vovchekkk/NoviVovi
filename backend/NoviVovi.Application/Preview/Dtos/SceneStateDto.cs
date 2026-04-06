using NoviVovi.Application.Dialogue.Dtos;
using NoviVovi.Application.Menu.Dtos;
using NoviVovi.Application.Scene.Dtos;

namespace NoviVovi.Application.Preview.Dtos;

public record SceneStateDto(
    BackgroundObjectDto? Background,
    ReplicaDto? Replica,
    MenuDto? Menu,
    IReadOnlyCollection<CharacterObjectDto> CharactersOnScene
);