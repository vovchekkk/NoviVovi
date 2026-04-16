using NoviVovi.Domain.Menu;
using NoviVovi.Domain.Transitions;
using NoviVovi.Infrastructure.DatabaseObjects.Choices;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class MenuMapper(LabelMapper labelMapper)
{
    public MenuDbO ToDbO(Menu stepMenu, Guid novelId)
    {
        var res = new MenuDbO
        {
            Id = stepMenu.Id,
            Name = stepMenu.Name,
            Description = stepMenu.Description,
            Choices = stepMenu.Choices.Select(choice => ToDbO(choice, novelId, stepMenu.Id)).ToList(),
        };
        return res;
    }

    public ChoiceDbO ToDbO(Choice choice, Guid novelId, Guid menuId)
    {
        var res = new ChoiceDbO
        {
            Id = choice.Id,
            Name = choice.Name,
            Text = choice.Text,
            NextLabelId = choice.Transition.TargetLabel.Id,
            NextLabel = labelMapper.ToDbO(choice.Transition.TargetLabel),
        };
        return res;
    }

    public Menu ToDomain(MenuDbO menu)
    {
        var res = new Menu(menu.Id, menu.Name, menu.Description, menu.Text);
        foreach (var choice in menu.Choices)
        {
            res.AddChoice(ToDomain(choice));
        }
        return res;
    }

    private Choice ToDomain(ChoiceDbO choice)
    {
        if (choice.NextLabel == null) throw new ArgumentException("Unsupported choice");
        var res = new Choice(choice.Id, choice.Name, null, choice.Text,
            new ChoiceTransition(labelMapper.ToDomain(choice.NextLabel)));
        return res;
    }
}