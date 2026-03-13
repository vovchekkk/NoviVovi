using NoviVovi.Api.Images.Responses;
using NoviVovi.Api.Scene.Responses;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public record ShowBackgroundStepResponse(
    Guid Id,
    ImageResponse Background,
    TransformResponse Transform,
    TransitionResponse Transition
) : StepResponse(Id, Transition);