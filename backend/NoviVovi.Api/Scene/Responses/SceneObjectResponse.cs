using System.Text.Json.Serialization;

namespace NoviVovi.Api.Scene.Responses;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(BackgroundObjectResponse), "background")]
[JsonDerivedType(typeof(CharacterObjectResponse), "character")]
public abstract record SceneObjectResponse(
    Guid Id,
    TransformResponse Transform
);