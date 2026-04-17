using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Menu;
using NoviVovi.Domain.Steps;
using NoviVovi.Infrastructure.DatabaseObjects.Choices;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;

namespace NoviVovi.Infrastructure.Mappers;

public class MappingContext
{
    public Dictionary<Guid, Label> Labels { get; } = new();
    public Dictionary<Guid, LabelDbO> LabelDbOs { get; } = new();

    public Dictionary<Guid, Step> Steps { get; } = new();
    public Dictionary<Guid, StepDbO> StepDbOs { get; } = new();

    public Dictionary<Guid, Menu> Menus { get; } = new();
    public Dictionary<Guid, MenuDbO> MenuDbOs { get; } = new();
}