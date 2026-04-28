using NoviVovi.Domain.Common;

namespace NoviVovi.Domain.Menu;

public class Menu : Entity
{
    private readonly List<Choice> _choices = new();
    
    public IReadOnlyList<Choice> Choices => _choices.AsReadOnly();

    public Menu(Guid id) : base(id)
    {
    }
    
    

    public static Menu Create()
    {
        return new Menu(Guid.NewGuid());
    }
    
    public static Menu Create(IEnumerable<Choice>? choices)
    {
        var menu = new Menu(Guid.NewGuid());
        
        if (choices is null)
            throw new DomainException($"Choices cannot be null");
        
        var choicesList = choices.ToList();
        if (choicesList.Count == 0)
            throw new DomainException($"Choices cannot be empty");
        
        menu.AddChoices(choicesList);
        
        return menu;
    }

    public void AddChoice(Choice? choice)
    {
        if (choice is null)
            throw new DomainException($"Choice cannot be null");
        
        if (_choices.Any(item => Equals(item, choice)))
            throw new DomainException($"Choice {choice.Id} already exists");
        
        _choices.Add(choice);
    }
    
    public void RemoveChoiceById(Guid choiceId)
    {
        if (choiceId == Guid.Empty)
            throw new DomainException($"StepId {choiceId} cannot be empty");

        var choice = _choices.FirstOrDefault(item => item.Id == choiceId);
        if (choice is null)
            throw new DomainException($"StepId {choiceId} doesn't exists");

        _choices.Remove(choice);
    }
    
    public void AddChoices(IEnumerable<Choice>? choices)
    {
        if (choices is null)
            throw new DomainException($"Choices cannot be null");
        
        _choices.AddRange(choices);
    }
    
    public void RemoveAllChoices()
    {
        _choices.Clear();
    }
}