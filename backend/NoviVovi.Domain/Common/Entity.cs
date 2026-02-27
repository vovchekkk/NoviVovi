namespace NoviVovi.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; protected set; }

    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
            throw new DomainException("Entity ID cannot be empty");

        Id = id;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other) return false;
        return Id == other.Id && GetType() == other.GetType();
    }

    public override int GetHashCode() => HashCode.Combine(Id, GetType().Name);
}