using NoviVovi.Domain.Common;
using NoviVovi.Domain.Slides;

namespace NoviVovi.Domain.Novels;

public class Novel : Entity
{
    private readonly List<Slide> _slides = new();
    public string Title { get; private set; }

    public IReadOnlyList<Slide> Slides => _slides.AsReadOnly();

    private Novel(Guid id, string title) : base(id)
    {
        Title = title;
    }

    public static Novel Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title cannot be empty");

        return new Novel(Guid.NewGuid(), title);
    }

    public static Novel Rehydrate(Guid id, string title, IEnumerable<Slide> slides)
    {
        var novel = new Novel(id, title); // приватный конструктор доступен внутри класса
        foreach (var s in slides)
            novel.AddSlide(s);
        return novel;
    }

    public void AddSlide(Slide slide)
    {
        if (_slides.Any(s => s.Number == slide.Number))
            throw new DomainException($"Slide {slide.Number} already exists");
        _slides.Add(slide);
    }

    public void RemoveSlide(int number)
    {
        var slide = _slides.FirstOrDefault(s => s.Number == number);
        if (slide == null)
            throw new DomainException($"Slide {number} does not exist");
        _slides.Remove(slide);
    }

    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title cannot be empty");
        Title = title;
    }
}