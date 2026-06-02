namespace BookingSystem.Modules.Reservations.UseCases.GetReservation;

internal sealed record GetReservationResponse(
    Guid ReservationId,
    Guid RoomId,
    string GuestName,
    DateTimeOffset Start,
    DateTimeOffset End,
    string Status,
    DateTimeOffset CreatedAt);
