namespace BookingSystem.Modules.Reservations.UseCases.ListReservations;

internal sealed record ReservationListItem(
    Guid ReservationId,
    Guid RoomId,
    string GuestName,
    DateTimeOffset Start,
    DateTimeOffset End,
    string Status);
