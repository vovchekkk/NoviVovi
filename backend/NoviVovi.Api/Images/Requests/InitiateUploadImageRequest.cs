using NoviVovi.Api.Scene.Requests;

namespace NoviVovi.Api.Images.Requests;

public record InitiateUploadImageRequest(
    string Name,
    string Format,
    ImageTypeRequest Type,
    SizeRequest Size
);