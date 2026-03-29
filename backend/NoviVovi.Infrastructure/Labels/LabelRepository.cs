using NoviVovi.Application.Labels;
using NoviVovi.Domain.Labels;

namespace NoviVovi.Infrastructure.Labels;

public class LabelRepository : ILabelRepository
{
    public async Task<Label?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(Label label)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Label label)
    {
        throw new NotImplementedException();
    }
}