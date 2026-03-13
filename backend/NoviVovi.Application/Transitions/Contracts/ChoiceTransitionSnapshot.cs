using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Transitions.Contracts;

public record ChoiceTransitionSnapshot(
    Guid Id,
    Guid TargetLabelId
) : TransitionSnapshot(Id);