using NoviVovi.Domain.Common;

namespace NoviVovi.Domain.Scene;

public class Size(int width, int height) : ValueObject
{
    public int Width { get; } = width;
    public int Height { get; } = height;
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Width;
        yield return Height;
    }
}