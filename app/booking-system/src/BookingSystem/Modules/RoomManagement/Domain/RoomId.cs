namespace BookingSystem.Modules.RoomManagement.Domain;

public sealed record RoomId(Guid Value)
{
    public static RoomId New() => new(Guid.NewGuid());
    public static RoomId From(Guid value) => new(value);
}
