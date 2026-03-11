using NoviVovi.Domain.Labels;

namespace NoviVovi.Application.Abstractions;

public interface ILabelRepository
{
    public Task<Label?> GetByIdAsync(Guid id);
    public Task AddAsync(Label novel);
    public Task DeleteAsync(Label novel);
}