using BookingSystem.BuildingBlocks.Domain;

namespace BookingSystem.Modules.RoomManagement.Domain;

internal sealed class RoomName : ValueObject
{
    public string Value { get; }

    private RoomName(string value) => Value = value;

    public static RoomName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Room name cannot be empty.");
        return new RoomName(value.Trim());
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
