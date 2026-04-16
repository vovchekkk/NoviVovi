using NoviVovi.Domain.Common;

namespace NoviVovi.Domain.Scene;

public class Position(double x, double y) : ValueObject
{
    public double X { get; } = x;
    public double Y { get; } = y;
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }
}