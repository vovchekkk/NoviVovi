using NoviVovi.Application.Abstractions;
using NoviVovi.Infrastructure.Persistence;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Infrastructure.Novels;

public class NovelRepository : INovelRepository
{
    private readonly AppDbContext _db;

    public NovelRepository(AppDbContext db) => _db = db;

    public async Task<Novel?> GetById(Guid id)
    {
        var dbModel = await _db.Novels.Include(n => n.Slides)
            .FirstOrDefaultAsync(n => n.Id == id);
        return dbModel?.ToDomain();
    }

    public async Task Save(Novel novel)
    {
        var dbModel = novel.ToDbModel();
        _db.Novels.Update(dbModel);
        await _db.SaveChangesAsync();
    }

    public async Task Delete(Novel novel)
    {
        var dbModel = novel.ToDbModel();
        _db.Novels.Remove(dbModel);
        await _db.SaveChangesAsync();
    }
}