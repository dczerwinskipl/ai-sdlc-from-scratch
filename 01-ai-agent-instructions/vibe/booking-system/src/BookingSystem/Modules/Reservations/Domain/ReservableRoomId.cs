using BookingSystem.BuildingBlocks.Domain;

namespace BookingSystem.Modules.Reservations.Domain;

internal sealed record ReservableRoomId(Guid Value)
{
    public static ReservableRoomId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new DomainException("Room id cannot be empty.");
        return new ReservableRoomId(value);
    }
}
