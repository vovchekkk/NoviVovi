using NoviVovi.Api.Menu.Responses;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public record ShowMenuStepResponse(
    Guid Id,
    MenuResponse Menu,
    TransitionResponse Transition
) : StepResponse(Id, Transition);