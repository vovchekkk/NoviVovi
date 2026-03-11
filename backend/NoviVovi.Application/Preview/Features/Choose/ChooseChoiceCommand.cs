namespace NoviVovi.Application.Preview.Features.Choose;

public record ChooseChoiceCommand(
    Guid SessionId,
    Guid ChoiceId
);