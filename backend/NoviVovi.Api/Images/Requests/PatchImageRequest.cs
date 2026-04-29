using NoviVovi.Api.Scene.Requests;

namespace NoviVovi.Api.Images.Requests;

public record PatchImageRequest(
    string? Name,
    ImageTypeRequest? Type
);