using System.Text.Json.Serialization;
using NoviVovi.Api.Common.Requests;

namespace NoviVovi.Api.Steps.Requests.Patch;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(PatchHideCharacterStepRequest), typeDiscriminator: "hide_character")]
[JsonDerivedType(typeof(PatchJumpStepRequest), typeDiscriminator: "jump")]
[JsonDerivedType(typeof(PatchShowBackgroundStepRequest), typeDiscriminator: "show_background")]
[JsonDerivedType(typeof(PatchShowCharacterStepRequest), typeDiscriminator: "show_character")]
[JsonDerivedType(typeof(PatchMenuStepRequest), typeDiscriminator: "menu")]
[JsonDerivedType(typeof(PatchReplicaStepRequest), typeDiscriminator: "replica")]
public abstract record PatchStepRequest(
    Guid NovelId,
    Guid LabelId,
    Guid StepId
);

public record PatchHideCharacterStepRequest(
    Guid NovelId,
    Guid LabelId,
    Guid StepId,
    Guid? CharacterId = null
) : PatchStepRequest(NovelId, LabelId, StepId);

public record PatchJumpStepRequest(
    Guid NovelId,
    Guid LabelId,
    Guid StepId,
    Guid? TargetLabelId = null
) : PatchStepRequest(NovelId, LabelId, StepId);

public record PatchShowBackgroundStepRequest(
    Guid NovelId,
    Guid LabelId,
    Guid StepId,
    Guid? ImageId = null,
    TransformPatchRequest? Transform = null
) : PatchStepRequest(NovelId, LabelId, StepId);

public record PatchShowCharacterStepRequest(
    Guid NovelId,
    Guid LabelId,
    Guid StepId,
    Guid? CharacterId = null,
    Guid? CharacterStateId = null,
    TransformPatchRequest? Transform = null
) : PatchStepRequest(NovelId, LabelId, StepId);

public record PatchMenuStepRequest(
    Guid NovelId,
    Guid LabelId,
    Guid StepId,
    string? Name = null,
    string? Description = null,
    string? Text = null
) : PatchStepRequest(NovelId, LabelId, StepId);

public record PatchReplicaStepRequest(
    Guid NovelId,
    Guid LabelId,
    Guid StepId,
    Guid? CharacterId = null,
    string? Text = null
) : PatchStepRequest(NovelId, LabelId, StepId);