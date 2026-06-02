namespace BookingSystem.Modules.Reservations.UseCases.ChangeReservationPeriod;

internal sealed record ChangeReservationPeriodRequest(DateTimeOffset Start, DateTimeOffset End);
