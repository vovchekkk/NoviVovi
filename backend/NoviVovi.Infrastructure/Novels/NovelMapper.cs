using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Slides;
using NoviVovi.Infrastructure.Slides;

namespace NoviVovi.Infrastructure.Novels;

public static class NovelMapper
{
    public static NovelDbModel ToDbModel(this Novel novel)
    {
        return new NovelDbModel
        {
            Id = novel.Id,
            Title = novel.Title,
            Slides = novel.Slides.Select(s => new SlideDbModel
            {
                Number = s.Number,
                Text = s.Text,
                NovelId = novel.Id
            }).ToList()
        };
    }

    public static Novel ToDomain(this NovelDbModel db)
    {
        var slides = db.Slides.Select(s => new Slide(s.Number, s.Text));
        return Novel.Rehydrate(db.Id, db.Title, slides);
    }
}