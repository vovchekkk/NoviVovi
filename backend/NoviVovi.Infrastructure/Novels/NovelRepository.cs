using NoviVovi.Application.Abstractions;
using NoviVovi.Infrastructure.Persistence;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Infrastructure.Novels;

public class NovelRepository(AppDbContext db) : INovelRepository
{
    public async Task<Novel?> GetById(Guid id)
    {
        var dbModel = await db.Novels.Include(n => n.Slides)
            .FirstOrDefaultAsync(n => n.Id == id);
        return dbModel?.ToDomain();
    }

    public async Task Save(Novel novel)
    {
        var dbModel = novel.ToDbModel();
        db.Novels.Update(dbModel);
        await db.SaveChangesAsync();
    }

    public async Task Delete(Novel novel)
    {
        var dbModel = novel.ToDbModel();
        db.Novels.Remove(dbModel);
        await db.SaveChangesAsync();
    }
}