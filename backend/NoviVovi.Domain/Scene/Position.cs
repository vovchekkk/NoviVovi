using NoviVovi.Domain.Common;

namespace NoviVovi.Domain.Scene;

public class Position(double x, double y) : ValueObject
{
    public double X { get; private set; } = x;
    public double Y { get; private set; } = y;
}