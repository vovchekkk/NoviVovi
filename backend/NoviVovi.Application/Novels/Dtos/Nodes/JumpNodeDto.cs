namespace NoviVovi.Application.Novels.Dtos.Nodes;

public record JumpNodeDto(
    Guid LabelId,
    string LabelName
) : NodeDto(LabelId, LabelName);
