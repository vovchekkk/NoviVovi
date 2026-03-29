using NoviVovi.Api.Common.Requests;

namespace NoviVovi.Api.Characters.Requests.Patch;

public record PatchCharacterStateRequest(
    Guid NovelId,
    Guid CharacterId,
    Guid CharacterStateId,
    TransformPatchRequest? Transform = null
);