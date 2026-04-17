using NoviVovi.Infrastructure.DatabaseObjects.Choices;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;

namespace NoviVovi.Infrastructure.Repositories;

public class LoadContext
{
    public Dictionary<Guid, LabelDbO> Labels { get; } = new();
    public Dictionary<Guid, StepDbO> Steps { get; } = new();
    public Dictionary<Guid, MenuDbO> Menus { get; } = new();
}