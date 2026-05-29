namespace BookingSystem.BuildingBlocks.Domain;

internal abstract class ValueObject
{
    protected abstract IEnumerable<object> GetAtomicValues();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType()) return false;
        return ((ValueObject)obj).GetAtomicValues().SequenceEqual(GetAtomicValues());
    }

    public override int GetHashCode() =>
        GetAtomicValues().Aggregate(0, HashCode.Combine);

    public static bool operator ==(ValueObject? left, ValueObject? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(ValueObject? left, ValueObject? right) =>
        !(left == right);
}
