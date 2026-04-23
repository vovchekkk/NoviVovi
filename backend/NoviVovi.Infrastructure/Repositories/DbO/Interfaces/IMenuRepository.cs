using NoviVovi.Infrastructure.DatabaseObjects.Choices;

namespace NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

public interface IMenuDbORepository
{
    Task<MenuDbO?> GetFullByIdAsync(Guid menuId, LoadContext ctx);
    Task<Guid> AddOrUpdateChoiceAsync(ChoiceDbO choice, LoadContext ctx);
    Task DeleteChoiceAsync(Guid menuId);
    Task DeleteAsync(Guid id);
    Task<Guid> AddOrUpdateFullAsync(MenuDbO stepMenu, LoadContext ctx);
}