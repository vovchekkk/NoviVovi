using NoviVovi.Api.Common.Requests;
using NoviVovi.Api.Images.Responses;

namespace NoviVovi.Api.Characters.Requests.Add;

public record AddCharacterStateRequest(
    Guid NovelId,
    Guid CharacterId,
    string Name,
    string? Description,
    Guid ImageId,
    TransformPatchRequest? Transform
);