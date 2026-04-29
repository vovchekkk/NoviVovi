using NoviVovi.Domain.Menu;
using NoviVovi.Domain.Transitions;
using NoviVovi.Infrastructure.DatabaseObjects.Choices;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class MenuMapper(
    Lazy<LabelMapper> labelMapper,
    MappingContext ctx
)
{
    public Menu ToDomain(MenuDbO menu)
    {
        var res = new Menu(menu.Id);
        foreach (var choice in menu.Choices)
        {
            res.AddChoice(ToDomain(choice));
        }

        return res;
    }

    public MenuDbO ToDbO(Menu stepMenu)
    {
        var res = new MenuDbO
        {
            Id = stepMenu.Id,
            Choices = stepMenu.Choices.Select(choice => ToDbO(choice, stepMenu.Id)).ToList(),
        };
        return res;
    }

    public Choice ToDomain(ChoiceDbO choice)
    {
        if (choice.NextLabel == null)
            throw new ArgumentException("NextLabel is null");

        return new Choice(
            choice.Id,
            choice.Text,
            new ChoiceTransition(labelMapper.Value.ToDomain(choice.NextLabel))
        );
    }

    public ChoiceDbO ToDbO(Choice choice, Guid menuId)
    {
        return new ChoiceDbO
        {
            Id = choice.Id,
            MenuId = menuId,
            Text = choice.Text,
            NextLabelId = choice.Transition.TargetLabel.Id,
            NextLabel = labelMapper.Value.ToDbO(choice.Transition.TargetLabel)
        };
    }
}