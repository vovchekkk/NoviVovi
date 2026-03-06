using NoviVovi.Domain.Common;

namespace NoviVovi.Domain.Menu;

public class Menu : Entity
{
    public string? Name { get; }
    public string? Description { get; }
    public string? Text { get; }
    private readonly List<Choice> _choices = new();
    
    public IReadOnlyList<Choice> Choices => _choices.AsReadOnly();
    
    private Menu(Guid id, string? name, string? description, string? text) : base(id)
    {
        Name = name;
        Description = description;
        Text = text;
    }

    public static Menu Create(string? name, string? description, string? text)
    {
        return new Menu(Guid.NewGuid(), name, description, text);
    }

    public static Menu Rehydrate(
        Guid id,
        string? name,
        string? description,
        string? text,
        IEnumerable<Choice> choices)
    {
        var menu = new Menu(id, name, description, text);
        foreach (var choice in choices)
            menu.AddChoice(choice);
        return menu;
    }

    public void AddChoice(Choice choice)
    {
        if (_choices.Any(item => item.Id == choice.Id))
            throw new DomainException($"Choice {choice.Id} already exists");
        _choices.Add(choice);
    }
}