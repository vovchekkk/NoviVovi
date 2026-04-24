using NoviVovi.Application.Menu.Dtos;

namespace NoviVovi.Application.Novels.Dtos.Nodes;

public record MenuNodeDto(
    Guid LabelId,
    string LabelName,
    List<ChoiceDto> Choices
) : NodeDto(LabelId, LabelName);
