namespace BookingSystem.Modules.Reservations.UseCases.CreateReservation;

internal sealed record CreateReservationRequest(
    Guid RoomId,
    string GuestName,
    DateTimeOffset Start,
    DateTimeOffset End);
