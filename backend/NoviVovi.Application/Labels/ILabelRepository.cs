using NoviVovi.Domain.Labels;

namespace NoviVovi.Application.Labels;

public interface ILabelRepository
{
    public Task<Label?> GetByIdAsync(Guid id);
    public Task AddAsync(Label label);
    public Task DeleteAsync(Label label);
}