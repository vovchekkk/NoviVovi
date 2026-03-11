using NoviVovi.Application.Dialogue.Contracts;
using NoviVovi.Application.Menu.Contracts;
using NoviVovi.Application.Scene.Contracts;

namespace NoviVovi.Application.Preview.Contracts;

public record SceneSnapshot(
    BackgroundObjectSnapshot? Background,
    ReplicaSnapshot? Replica,
    MenuSnapshot? Menu,
    Dictionary<Guid, CharacterObjectSnapshot> Characters
);