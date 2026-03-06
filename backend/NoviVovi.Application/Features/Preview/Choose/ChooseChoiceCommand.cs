namespace NoviVovi.Application.Features.Preview.Choose;

public record ChooseChoiceCommand(
    Guid SessionId,
    Guid ChoiceId
);