using NoviVovi.Domain.Menu;
using NoviVovi.Domain.Transitions;
using NoviVovi.Infrastructure.DatabaseObjects.Choices;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class MenuMapper(
    Lazy<LabelMapper> labelMapper
)
{
    public MenuDbO ToDbO(Menu stepMenu, Guid novelId)
    {
        var res = new MenuDbO
        {
            Id = stepMenu.Id,
            Choices = stepMenu.Choices.Select(choice => ToDbO(choice, novelId, stepMenu.Id, new MappingContext())).ToList(),
        };
        return res;
    }

    public Menu ToDomain(MenuDbO menu)
    {
        var res = new Menu(menu.Id);
        foreach (var choice in menu.Choices)
        {
            res.AddChoice(ToDomain(choice, new MappingContext()));
        }

        return res;
    }
    
    public ChoiceDbO ToDbO(Choice choice, Guid novelId, Guid menuId, MappingContext ctx)
    {
        return new ChoiceDbO
        {
            Id = choice.Id,
            Text = choice.Text,
            MenuId = menuId,
            NextLabelId = choice.Transition.TargetLabel.Id,
            NextLabel = labelMapper.Value.ToDbO(choice.Transition.TargetLabel, ctx)
        };
    }

    private Choice ToDomain(ChoiceDbO choice, MappingContext ctx)
    {
        if (choice.NextLabel == null)
            throw new ArgumentException("Unsupported choice");

        return new Choice(
            choice.Id,
            choice.Text,
            new ChoiceTransition(labelMapper.Value.ToDomain(choice.NextLabel, ctx))
        );
    }
}