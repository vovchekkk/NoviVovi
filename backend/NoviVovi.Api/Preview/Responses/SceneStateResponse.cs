using NoviVovi.Api.Dialogue.Responses;
using NoviVovi.Api.Menu.Responses;
using NoviVovi.Api.Scene.Responses;

namespace NoviVovi.Api.Preview.Responses;

public record SceneStateResponse(
    BackgroundObjectResponse? Background,
    ReplicaResponse? Replica,
    MenuResponse? Menu,
    IReadOnlyCollection<CharacterObjectResponse> CharactersOnScene
);