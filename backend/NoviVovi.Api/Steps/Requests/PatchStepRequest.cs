using System.Text.Json.Serialization;
using NoviVovi.Api.Menu.Requests;
using NoviVovi.Api.Scene.Requests;

namespace NoviVovi.Api.Steps.Requests;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(PatchHideCharacterStepRequest), typeDiscriminator: "hide_character")]
[JsonDerivedType(typeof(PatchJumpStepRequest), typeDiscriminator: "jump")]
[JsonDerivedType(typeof(PatchShowBackgroundStepRequest), typeDiscriminator: "show_background")]
[JsonDerivedType(typeof(PatchShowCharacterStepRequest), typeDiscriminator: "show_character")]
[JsonDerivedType(typeof(PatchShowMenuStepRequest), typeDiscriminator: "menu")]
[JsonDerivedType(typeof(PatchShowReplicaStepRequest), typeDiscriminator: "replica")]
public abstract record PatchStepRequest;

public record PatchHideCharacterStepRequest(
    Guid? CharacterId = null
) : PatchStepRequest;

public record PatchJumpStepRequest(
    Guid? TargetLabelId = null
) : PatchStepRequest;

public record PatchShowBackgroundStepRequest(
    Guid? ImageId = null,
    TransformRequest? Transform = null
) : PatchStepRequest;

public record PatchShowCharacterStepRequest(
    Guid? CharacterId = null,
    Guid? CharacterStateId = null,
    TransformRequest? Transform = null
) : PatchStepRequest;

public record PatchShowMenuStepRequest(
    IEnumerable<ChoiceRequest>? Choices = null
) : PatchStepRequest;

public record PatchShowReplicaStepRequest(
    Guid? CharacterId = null,
    string? Text = null
) : PatchStepRequest;