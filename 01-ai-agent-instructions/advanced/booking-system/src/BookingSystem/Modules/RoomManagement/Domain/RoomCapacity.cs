using BookingSystem.BuildingBlocks.Domain;

namespace BookingSystem.Modules.RoomManagement.Domain;

internal sealed class RoomCapacity : ValueObject
{
    public int Value { get; }

    private RoomCapacity(int value) => Value = value;

    public static RoomCapacity Create(int value)
    {
        if (value <= 0)
            throw new DomainException("Room capacity must be greater than 0.");
        return new RoomCapacity(value);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
