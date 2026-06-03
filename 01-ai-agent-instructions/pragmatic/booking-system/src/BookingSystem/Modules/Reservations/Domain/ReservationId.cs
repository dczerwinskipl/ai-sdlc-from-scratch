namespace BookingSystem.Modules.Reservations.Domain;

internal sealed record ReservationId(Guid Value)
{
    public static ReservationId New() => new(Guid.NewGuid());
    public static ReservationId From(Guid value) => new(value);
}
