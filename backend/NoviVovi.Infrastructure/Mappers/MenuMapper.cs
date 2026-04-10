using NoviVovi.Domain.Menu;
using NoviVovi.Infrastructure.DatabaseObjects.Choices;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class MenuMapper
{
    public MenuDbO ToDbO(Menu stepMenu)
    {
        throw new NotImplementedException();
    }

    public Menu ToDomain(MenuDbO stepMenu)
    {
        throw new NotImplementedException();
    }
}