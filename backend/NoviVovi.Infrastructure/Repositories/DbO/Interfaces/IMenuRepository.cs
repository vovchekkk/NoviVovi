using NoviVovi.Infrastructure.DatabaseObjects.Choices;

namespace NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

public interface IMenuDbORepository
{
    Task<MenuDbO?> GetFullByIdAsync(Guid menuId);

    Task<Guid> AddAsync(MenuDbO menu);
    Task<Guid> AddChoiceAsync(ChoiceDbO choice);
    Task UpdateAsync(MenuDbO menu);
    Task DeleteAsync(Guid id);
    Task AddFullAsync(MenuDbO stepMenu);
}