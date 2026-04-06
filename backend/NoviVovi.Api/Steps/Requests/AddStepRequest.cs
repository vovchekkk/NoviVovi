using System.Text.Json.Serialization;
using NoviVovi.Api.Menu.Requests;
using NoviVovi.Api.Scene.Requests;

namespace NoviVovi.Api.Steps.Requests;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(AddHideCharacterStepRequest), typeDiscriminator: "hide_character")]
[JsonDerivedType(typeof(AddJumpStepRequest), typeDiscriminator: "jump")]
[JsonDerivedType(typeof(AddShowBackgroundStepRequest), typeDiscriminator: "show_background")]
[JsonDerivedType(typeof(AddShowCharacterStepRequest), typeDiscriminator: "show_character")]
[JsonDerivedType(typeof(AddShowMenuStepRequest), typeDiscriminator: "menu")]
[JsonDerivedType(typeof(AddShowReplicaStepRequest), typeDiscriminator: "replica")]
public abstract record AddStepRequest;

public record AddHideCharacterStepRequest(
    Guid CharacterId
) : AddStepRequest;

public record AddJumpStepRequest(
    Guid TargetLabelId
) : AddStepRequest;

public record AddShowBackgroundStepRequest(
    Guid ImageId,
    TransformRequest Transform
) : AddStepRequest;

public record AddShowCharacterStepRequest(
    Guid CharacterId,
    Guid CharacterStateId,
    TransformRequest Transform
) : AddStepRequest;

public record AddShowMenuStepRequest(
    string Name,
    string? Description,
    string Text,
    MenuRequest Menu
) : AddStepRequest;

public record AddShowReplicaStepRequest(
    Guid CharacterId,
    string Text
) : AddStepRequest;