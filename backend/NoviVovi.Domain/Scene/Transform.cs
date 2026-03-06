using NoviVovi.Domain.Common;

namespace NoviVovi.Domain.Scene;

public class Transform : ValueObject
{
    public Position Position { get; set; } = new(0, 0);
    public Size Size { get; set; } = new(0, 0);
    public double Scale { get; set; } = 1.0;
    public double Rotation { get; set; }
    public int ZIndex { get; set; }
}