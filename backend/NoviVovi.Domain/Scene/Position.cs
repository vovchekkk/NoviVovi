using NoviVovi.Domain.Common;

namespace NoviVovi.Domain.Scene;

public class Position(double x = 0.0, double y = 0.0) : ValueObject
{
    public double X { get; } = x;
    public double Y { get; } = y;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }
}