using System.Text.Json.Serialization;

namespace NoviVovi.Api.Steps.Requests.Add;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(AddHideCharacterStepRequest), typeDiscriminator: "hide_character")]
[JsonDerivedType(typeof(AddJumpStepRequest), typeDiscriminator: "jump")]
[JsonDerivedType(typeof(AddShowBackgroundStepRequest), typeDiscriminator: "show_background")]
[JsonDerivedType(typeof(AddShowCharacterStepRequest), typeDiscriminator: "show_character")]
[JsonDerivedType(typeof(AddMenuStepRequest), typeDiscriminator: "menu")]
[JsonDerivedType(typeof(AddReplicaStepRequest), typeDiscriminator: "replica")]
public abstract record AddStepRequest(
    Guid NovelId,
    Guid LabelId
);

public record AddHideCharacterStepRequest(
    Guid NovelId,
    Guid LabelId,
    Guid CharacterId
) : AddStepRequest(NovelId, LabelId);

public record AddJumpStepRequest(
    Guid NovelId,
    Guid LabelId,
    Guid TargetLabelId
) : AddStepRequest(NovelId, LabelId);

public record AddShowBackgroundStepRequest(
    Guid NovelId,
    Guid LabelId,
    Guid ImageId,
    double X,
    double Y,
    int Width,
    int Height,
    double Scale,
    double Rotation,
    int ZIndex
) : AddStepRequest(NovelId, LabelId);

public record AddShowCharacterStepRequest(
    Guid NovelId,
    Guid LabelId,
    Guid CharacterId,
    Guid CharacterStateId,
    double X,
    double Y,
    int Width,
    int Height,
    double Scale,
    double Rotation,
    int ZIndex
) : AddStepRequest(NovelId, LabelId);

public record AddMenuStepRequest(
    Guid NovelId,
    Guid LabelId,
    string Name,
    string? Description,
    string Text
) : AddStepRequest(NovelId, LabelId);

public record AddReplicaStepRequest(
    Guid NovelId,
    Guid LabelId,
    Guid CharacterId,
    string Text
) : AddStepRequest(NovelId, LabelId);