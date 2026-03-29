using NoviVovi.Domain.Common;

namespace NoviVovi.Domain.Scene;

public class Size(int width, int height) : ValueObject
{
    public int Width { get; set; } = width;
    public int Height { get; set; } = height;
}